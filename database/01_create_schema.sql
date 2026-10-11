-- =============================================================
-- Controle de Vendas — script de criação do banco de dados
-- Banco: PostgreSQL 14+
-- Convenção: snake_case para objetos do banco, valores monetários
-- em NUMERIC(18,2) para precisão exata.
-- =============================================================

CREATE DATABASE sales_control;

\c sales_control;

BEGIN;

-- -------------------------------------------------------------
-- customers
-- CPF é a chave de negócio: índice único garante a validação de
-- duplicidade exigida no escopo, além da checagem na aplicação.
-- -------------------------------------------------------------
CREATE TABLE customers (
    id          BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    cpf         VARCHAR(11)  NOT NULL,
    name        VARCHAR(200) NOT NULL,
    birth_date  DATE         NULL,
    email       VARCHAR(200) NULL,
    phone       VARCHAR(20)  NULL,
    created_at  TIMESTAMPTZ  NOT NULL DEFAULT now(),
    updated_at  TIMESTAMPTZ  NOT NULL DEFAULT now(),

    CONSTRAINT ck_customers_cpf_digits_only CHECK (cpf ~ '^[0-9]{11}$')
);

-- Validação de duplicidade de CPF (requisito) — índice único também
-- otimiza a busca do cliente pelo CPF na tela de vendas.
CREATE UNIQUE INDEX ux_customers_cpf ON customers (cpf);

-- Apoia a busca por nome na tela de vendas (ILIKE 'termo%').
CREATE INDEX ix_customers_name ON customers (name);


-- -------------------------------------------------------------
-- products
-- Nome é único por requisito de validação de duplicidade.
-- -------------------------------------------------------------
CREATE TABLE products (
    id          BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    name        VARCHAR(200)     NOT NULL,
    unit_price  NUMERIC(18, 2)   NOT NULL,
    created_at  TIMESTAMPTZ      NOT NULL DEFAULT now(),
    updated_at  TIMESTAMPTZ      NOT NULL DEFAULT now(),

    CONSTRAINT ck_products_unit_price_non_negative CHECK (unit_price >= 0)
);

CREATE UNIQUE INDEX ux_products_name ON products (name);


-- -------------------------------------------------------------
-- sales
-- Cabeçalho da operação. total_amount é desnormalizado de forma
-- controlada (calculado pela aplicação na gravação) para leitura
-- rápida em listagens — os itens permanecem a fonte da verdade.
-- -------------------------------------------------------------
CREATE TABLE sales (
    id            BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    customer_id   BIGINT          NOT NULL,
    sale_date     DATE            NOT NULL DEFAULT CURRENT_DATE,
    total_amount  NUMERIC(18, 2)  NOT NULL DEFAULT 0,
    created_at    TIMESTAMPTZ     NOT NULL DEFAULT now(),

    CONSTRAINT fk_sales_customer
        FOREIGN KEY (customer_id) REFERENCES customers (id)
        ON DELETE RESTRICT
);

CREATE INDEX ix_sales_customer   ON sales (customer_id);
CREATE INDEX ix_sales_date       ON sales (sale_date);


-- -------------------------------------------------------------
-- sale_items
-- unit_price é copiado do produto no momento da venda para
-- preservar o histórico: alterações futuras de preço no cadastro
-- não afetam vendas já registradas.
-- -------------------------------------------------------------
CREATE TABLE sale_items (
    id          BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    sale_id     BIGINT          NOT NULL,
    product_id  BIGINT          NOT NULL,
    quantity    INTEGER         NOT NULL,
    unit_price  NUMERIC(18, 2)  NOT NULL,
    item_total  NUMERIC(18, 2)  GENERATED ALWAYS AS (quantity * unit_price) STORED,

    CONSTRAINT fk_sale_items_sale
        FOREIGN KEY (sale_id) REFERENCES sales (id)
        ON DELETE CASCADE,
    CONSTRAINT fk_sale_items_product
        FOREIGN KEY (product_id) REFERENCES products (id)
        ON DELETE RESTRICT,
    CONSTRAINT ck_sale_items_quantity_positive        CHECK (quantity > 0),
    CONSTRAINT ck_sale_items_unit_price_non_negative  CHECK (unit_price >= 0)
);

CREATE INDEX ix_sale_items_sale     ON sale_items (sale_id);
CREATE INDEX ix_sale_items_product  ON sale_items (product_id);

COMMIT;
