CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261009143922_InitialCreate') THEN
    CREATE TABLE clientes (
        id bigint GENERATED ALWAYS AS IDENTITY,
        cpf character varying(11) NOT NULL,
        nome character varying(200) NOT NULL,
        data_nascimento date,
        email character varying(200),
        telefone character varying(20),
        criado_em timestamp with time zone NOT NULL DEFAULT (now()),
        atualizado_em timestamp with time zone NOT NULL DEFAULT (now()),
        CONSTRAINT "PK_clientes" PRIMARY KEY (id),
        CONSTRAINT ck_clientes_cpf_apenas_digitos CHECK (cpf ~ '^[0-9]{11}$')
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261009143922_InitialCreate') THEN
    CREATE TABLE produtos (
        id bigint GENERATED ALWAYS AS IDENTITY,
        nome character varying(200) NOT NULL,
        valor_unitario numeric(18,2) NOT NULL,
        criado_em timestamp with time zone NOT NULL DEFAULT (now()),
        atualizado_em timestamp with time zone NOT NULL DEFAULT (now()),
        CONSTRAINT "PK_produtos" PRIMARY KEY (id),
        CONSTRAINT ck_produtos_valor_positivo CHECK (valor_unitario >= 0)
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261009143922_InitialCreate') THEN
    CREATE TABLE vendas (
        id bigint GENERATED ALWAYS AS IDENTITY,
        cliente_id bigint NOT NULL,
        data_venda date NOT NULL DEFAULT (CURRENT_DATE),
        valor_total numeric(18,2) NOT NULL DEFAULT 0.0,
        criado_em timestamp with time zone NOT NULL DEFAULT (now()),
        CONSTRAINT "PK_vendas" PRIMARY KEY (id),
        CONSTRAINT "FK_vendas_clientes_cliente_id" FOREIGN KEY (cliente_id) REFERENCES clientes (id) ON DELETE RESTRICT
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261009143922_InitialCreate') THEN
    CREATE TABLE itens_venda (
        id bigint GENERATED ALWAYS AS IDENTITY,
        venda_id bigint NOT NULL,
        produto_id bigint NOT NULL,
        quantidade integer NOT NULL,
        preco_unitario numeric(18,2) NOT NULL,
        total_item numeric(18,2) GENERATED ALWAYS AS (quantidade * preco_unitario) STORED NOT NULL,
        CONSTRAINT "PK_itens_venda" PRIMARY KEY (id),
        CONSTRAINT ck_itens_preco_positivo CHECK (preco_unitario >= 0),
        CONSTRAINT ck_itens_quantidade_positiva CHECK (quantidade > 0),
        CONSTRAINT "FK_itens_venda_produtos_produto_id" FOREIGN KEY (produto_id) REFERENCES produtos (id) ON DELETE RESTRICT,
        CONSTRAINT "FK_itens_venda_vendas_venda_id" FOREIGN KEY (venda_id) REFERENCES vendas (id) ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261009143922_InitialCreate') THEN
    CREATE INDEX ix_clientes_nome ON clientes (nome);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261009143922_InitialCreate') THEN
    CREATE UNIQUE INDEX ux_clientes_cpf ON clientes (cpf);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261009143922_InitialCreate') THEN
    CREATE INDEX ix_itens_produto ON itens_venda (produto_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261009143922_InitialCreate') THEN
    CREATE INDEX ix_itens_venda ON itens_venda (venda_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261009143922_InitialCreate') THEN
    CREATE UNIQUE INDEX ux_produtos_nome ON produtos (nome);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261009143922_InitialCreate') THEN
    CREATE INDEX ix_vendas_cliente ON vendas (cliente_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261009143922_InitialCreate') THEN
    CREATE INDEX ix_vendas_data ON vendas (data_venda);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261009143922_InitialCreate') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20261009143922_InitialCreate', '8.0.31');
    END IF;
END $EF$;
COMMIT;

