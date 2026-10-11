CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261011225456_InitialCreate') THEN
    CREATE TABLE customers (
        id bigint GENERATED ALWAYS AS IDENTITY,
        cpf character varying(11) NOT NULL,
        name character varying(200) NOT NULL,
        birth_date date,
        email character varying(200),
        phone character varying(20),
        created_at timestamp with time zone NOT NULL DEFAULT (now()),
        updated_at timestamp with time zone NOT NULL DEFAULT (now()),
        CONSTRAINT "PK_customers" PRIMARY KEY (id),
        CONSTRAINT ck_customers_cpf_digits_only CHECK (cpf ~ '^[0-9]{11}$')
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261011225456_InitialCreate') THEN
    CREATE TABLE products (
        id bigint GENERATED ALWAYS AS IDENTITY,
        name character varying(200) NOT NULL,
        unit_price numeric(18,2) NOT NULL,
        created_at timestamp with time zone NOT NULL DEFAULT (now()),
        updated_at timestamp with time zone NOT NULL DEFAULT (now()),
        CONSTRAINT "PK_products" PRIMARY KEY (id),
        CONSTRAINT ck_products_unit_price_non_negative CHECK (unit_price >= 0)
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261011225456_InitialCreate') THEN
    CREATE TABLE sales (
        id bigint GENERATED ALWAYS AS IDENTITY,
        customer_id bigint NOT NULL,
        sale_date date NOT NULL DEFAULT (CURRENT_DATE),
        total_amount numeric(18,2) NOT NULL DEFAULT 0.0,
        created_at timestamp with time zone NOT NULL DEFAULT (now()),
        CONSTRAINT "PK_sales" PRIMARY KEY (id),
        CONSTRAINT "FK_sales_customers_customer_id" FOREIGN KEY (customer_id) REFERENCES customers (id) ON DELETE RESTRICT
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261011225456_InitialCreate') THEN
    CREATE TABLE sale_items (
        id bigint GENERATED ALWAYS AS IDENTITY,
        sale_id bigint NOT NULL,
        product_id bigint NOT NULL,
        quantity integer NOT NULL,
        unit_price numeric(18,2) NOT NULL,
        item_total numeric(18,2) GENERATED ALWAYS AS (quantity * unit_price) STORED NOT NULL,
        CONSTRAINT "PK_sale_items" PRIMARY KEY (id),
        CONSTRAINT ck_sale_items_quantity_positive CHECK (quantity > 0),
        CONSTRAINT ck_sale_items_unit_price_non_negative CHECK (unit_price >= 0),
        CONSTRAINT "FK_sale_items_products_product_id" FOREIGN KEY (product_id) REFERENCES products (id) ON DELETE RESTRICT,
        CONSTRAINT "FK_sale_items_sales_sale_id" FOREIGN KEY (sale_id) REFERENCES sales (id) ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261011225456_InitialCreate') THEN
    CREATE INDEX ix_customers_name ON customers (name);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261011225456_InitialCreate') THEN
    CREATE UNIQUE INDEX ux_customers_cpf ON customers (cpf);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261011225456_InitialCreate') THEN
    CREATE UNIQUE INDEX ux_products_name ON products (name);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261011225456_InitialCreate') THEN
    CREATE INDEX ix_sale_items_product ON sale_items (product_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261011225456_InitialCreate') THEN
    CREATE INDEX ix_sale_items_sale ON sale_items (sale_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261011225456_InitialCreate') THEN
    CREATE INDEX ix_sales_customer ON sales (customer_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261011225456_InitialCreate') THEN
    CREATE INDEX ix_sales_date ON sales (sale_date);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261011225456_InitialCreate') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20261011225456_InitialCreate', '8.0.31');
    END IF;
END $EF$;
COMMIT;

