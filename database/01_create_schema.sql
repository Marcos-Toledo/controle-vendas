-- =============================================================
-- Controle de Vendas — script de criação do banco de dados
-- Banco: PostgreSQL 14+
-- Convenção: snake_case para objetos do banco, valores monetários
-- em NUMERIC(18,2) para precisão exata.
-- =============================================================

CREATE DATABASE controle_vendas;

\c controle_vendas;

BEGIN;

-- -------------------------------------------------------------
-- clientes
-- CPF é a chave de negócio: índice único garante a validação de
-- duplicidade exigida no escopo, além da checagem na aplicação.
-- -------------------------------------------------------------
CREATE TABLE clientes (
    id               BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    cpf              VARCHAR(11)  NOT NULL,
    nome             VARCHAR(200) NOT NULL,
    data_nascimento  DATE         NULL,
    email            VARCHAR(200) NULL,
    telefone         VARCHAR(20)  NULL,
    criado_em        TIMESTAMPTZ  NOT NULL DEFAULT now(),
    atualizado_em    TIMESTAMPTZ  NOT NULL DEFAULT now(),

    CONSTRAINT ck_clientes_cpf_apenas_digitos CHECK (cpf ~ '^[0-9]{11}$')
);

-- Validação de duplicidade de CPF (requisito) — índice único também
-- otimiza a busca do cliente pelo CPF na tela de vendas.
CREATE UNIQUE INDEX ux_clientes_cpf ON clientes (cpf);

-- Apoia a busca por nome na tela de vendas (ILIKE 'termo%').
CREATE INDEX ix_clientes_nome ON clientes (nome);


-- -------------------------------------------------------------
-- produtos
-- Nome é único por requisito de validação de duplicidade.
-- -------------------------------------------------------------
CREATE TABLE produtos (
    id              BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    nome            VARCHAR(200)     NOT NULL,
    valor_unitario  NUMERIC(18, 2)   NOT NULL,
    criado_em       TIMESTAMPTZ      NOT NULL DEFAULT now(),
    atualizado_em   TIMESTAMPTZ      NOT NULL DEFAULT now(),

    CONSTRAINT ck_produtos_valor_positivo CHECK (valor_unitario >= 0)
);

CREATE UNIQUE INDEX ux_produtos_nome ON produtos (nome);


-- -------------------------------------------------------------
-- vendas
-- Cabeçalho da operação. valor_total é desnormalizado de forma
-- controlada (calculado pela aplicação na gravação) para leitura
-- rápida em listagens — os itens permanecem a fonte da verdade.
-- -------------------------------------------------------------
CREATE TABLE vendas (
    id           BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    cliente_id   BIGINT          NOT NULL,
    data_venda   DATE            NOT NULL DEFAULT CURRENT_DATE,
    valor_total  NUMERIC(18, 2)  NOT NULL DEFAULT 0,
    criado_em    TIMESTAMPTZ     NOT NULL DEFAULT now(),

    CONSTRAINT fk_vendas_cliente
        FOREIGN KEY (cliente_id) REFERENCES clientes (id)
        ON DELETE RESTRICT
);

CREATE INDEX ix_vendas_cliente   ON vendas (cliente_id);
CREATE INDEX ix_vendas_data      ON vendas (data_venda);


-- -------------------------------------------------------------
-- itens_venda
-- preco_unitario é copiado do produto no momento da venda para
-- preservar o histórico: alterações futuras de preço no cadastro
-- não afetam vendas já registradas.
-- -------------------------------------------------------------
CREATE TABLE itens_venda (
    id             BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    venda_id       BIGINT          NOT NULL,
    produto_id     BIGINT          NOT NULL,
    quantidade     INTEGER         NOT NULL,
    preco_unitario NUMERIC(18, 2)  NOT NULL,
    total_item     NUMERIC(18, 2)  GENERATED ALWAYS AS (quantidade * preco_unitario) STORED,

    CONSTRAINT fk_itens_venda
        FOREIGN KEY (venda_id) REFERENCES vendas (id)
        ON DELETE CASCADE,
    CONSTRAINT fk_itens_produto
        FOREIGN KEY (produto_id) REFERENCES produtos (id)
        ON DELETE RESTRICT,
    CONSTRAINT ck_itens_quantidade_positiva CHECK (quantidade > 0),
    CONSTRAINT ck_itens_preco_positivo      CHECK (preco_unitario >= 0)
);

CREATE INDEX ix_itens_venda   ON itens_venda (venda_id);
CREATE INDEX ix_itens_produto ON itens_venda (produto_id);

COMMIT;
