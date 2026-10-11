# Controle de Vendas

Sistema web para controle de vendas: cadastro de **clientes**, **produtos** e
lançamento de **vendas** com múltiplos itens.

Desenvolvido como teste prático — requisitos no escopo do desafio.

> **Convenção**: o código-fonte (backend, frontend e schema do banco) está em
> inglês; mensagens de erro, textos da interface e esta documentação estão em
> pt-BR.

## Stack

| Camada    | Tecnologia                                              |
| --------- | ------------------------------------------------------- |
| Frontend  | React 19 + TypeScript + Vite + Tailwind CSS + Router    |
| Backend   | .NET 8 (ASP.NET Core, C#) + Entity Framework Core 8     |
| Banco     | PostgreSQL 16 (SQL Server/MySQL seriam equivalentes)    |
| Testes    | xUnit                                                   |
| API Docs  | Swagger / OpenAPI (Swashbuckle)                         |

> **Por que PostgreSQL?** O escopo permite SQL Server (preferencial), PostgreSQL
> ou MySQL. Escolhi PostgreSQL por já rodar em container Docker no ambiente de
> desenvolvimento, sem licença e multiplataforma. Todo o acesso a dados passa
> pelo EF Core, então trocar o provider (Npgsql → SqlServer) é questão de uma
> linha em `Program.cs` + ajuste de sintaxe no script SQL.

## Arquitetura

Backend em camadas seguindo Clean Architecture e SOLID:

```
backend/
├── SalesControl.sln
├── src/
│   ├── SalesControl.Domain/          # entidades e regras de negócio puras
│   │   └── Entities/                 #   Customer, Product, Sale, SaleItem
│   ├── SalesControl.Application/     # casos de uso, DTOs, validações
│   │   ├── DTOs/
│   │   ├── Interfaces/               #   portas (repositórios, IUnitOfWork)
│   │   └── Services/
│   ├── SalesControl.Infrastructure/  # EF Core, DbContext, repositórios
│   │   ├── Persistence/Configurations/ #   mapeamentos Fluent API
│   │   ├── Persistence/Migrations/     #   InitialCreate (paridade com o script)
│   │   └── Repositories/
│   └── SalesControl.Api/             # controllers, middlewares, Swagger
└── tests/
    └── SalesControl.Tests/           # xUnit — regras de negócio
```

Direção das dependências: `Api → Application → Domain` e
`Api → Infrastructure → Application`. O domínio não conhece EF Core nem HTTP.

## Banco de dados

Modelo normalizado (3FN) com chaves primárias, estrangeiras, índices e
constraints de obrigatoriedade — script completo em
[`database/01_create_schema.sql`](database/01_create_schema.sql).

```
customers ──────< sales ──────< sale_items >────── products
```

Resumo do schema:

```
customers  (id PK, cpf UNIQUE NOT NULL, name NOT NULL,
            birth_date, email, phone,
            created_at, updated_at)

products   (id PK, name UNIQUE NOT NULL, unit_price NUMERIC(18,2) NOT NULL,
            created_at, updated_at)

sales      (id PK, customer_id FK NOT NULL, sale_date NOT NULL,
            total_amount NUMERIC(18,2) NOT NULL, created_at)

sale_items (id PK, sale_id FK NOT NULL, product_id FK NOT NULL,
            quantity INT NOT NULL, unit_price NUMERIC(18,2) NOT NULL,
            item_total NUMERIC(18,2) GENERATED ALWAYS AS (quantity * unit_price))
```

Decisões de modelagem:

- **`customers.cpf` com índice único** — a validação de duplicidade exigida no
  escopo existe no banco, não só na aplicação. O índice também acelera a busca
  por CPF na tela de vendas.
- **`products.name` com índice único** — mesma regra de duplicidade.
- **`sale_items.unit_price` gravado no item** — preserva o preço do
  momento da venda; mudanças futuras no cadastro do produto não alteram o
  histórico.
- **`sale_items.item_total` coluna gerada** (`quantity * unit_price`) —
  total por item garantido pelo banco.
- **`sales.total_amount` desnormalizado de forma controlada** — calculado pela
  aplicação na gravação, agiliza listagens; os itens continuam a fonte da
  verdade.
- **`ON DELETE RESTRICT` em cliente/produto** usados em vendas — preserva a
  integridade do histórico; `CASCADE` apenas nos itens da própria venda.
- **CHECKs** de CPF (11 dígitos), quantidade > 0 e valores ≥ 0.

## Como rodar

### Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download) — o projeto mira
  `net8.0`. Um SDK mais novo (9/10) também compila, mas exige o **runtime 8**
  instalado para rodar/testar (ou `export DOTNET_ROLL_FORWARD=LatestMajor`)
- [Node.js 20+](https://nodejs.org) e [pnpm](https://pnpm.io) (`npm` também
  funciona, trocando `pnpm` por `npm` nos comandos)
- [Docker](https://www.docker.com) para o PostgreSQL — ou uma instância própria

### 1. Banco de dados

Com Docker (recomendado — sobe o Postgres e já executa o script de criação):

```bash
docker compose up -d
```

Isso sobe o PostgreSQL na porta **5433** e executa
`database/01_create_schema.sql` automaticamente.

Alternativa sem Docker: com um PostgreSQL rodando, execute o script manualmente:

```bash
psql -U postgres -f database/01_create_schema.sql
```

Também há uma migration EF (`InitialCreate`) com paridade total ao script —
num banco **vazio**, `dotnet ef database update` gera o mesmo schema
(requer `dotnet tool restore` no diretório `backend/`). Use **ou** o script
**ou** a migration, nunca os dois.

Para referência, o SQL gerado pela migration está versionado em
[`database/02_ef_migration_script.sql`](database/02_ef_migration_script.sql)
(script idempotente — pode ser executado com `psql -f` num banco vazio em
vez de `dotnet ef database update`).

Ajuste a connection string em
`backend/src/SalesControl.Api/appsettings.json` se necessário.

### 2. Backend

```bash
cd backend
dotnet restore
dotnet run --project src/SalesControl.Api
```

API em `http://localhost:5125` — Swagger UI em
`http://localhost:5125/swagger`.

> **Erro `You must install or update .NET to run this application`?**
> Significa que só há runtime mais novo instalado (o app mira `net8.0`).
> Instale o [.NET 8 Runtime](https://dotnet.microsoft.com/download/dotnet/8.0)
> **ou** rode com `DOTNET_ROLL_FORWARD=LatestMajor dotnet run --project src/SalesControl.Api`.

### 3. Frontend

```bash
cd frontend
pnpm install
pnpm dev
```

App em `http://localhost:5173` — as chamadas `/api/*` são redirecionadas à API
pelo proxy do Vite.

### 4. Testes

```bash
cd backend
dotnet test
```

### Problemas comuns

- **`Failed to bind to address … address already in use`** — já existe outra
  instância da API (5125) ou do Vite (5173) rodando; encerre-a antes de subir
  uma nova.
- **API sobe mas falha ao acessar o banco** — o Postgres não está no ar:
  `docker compose up -d` e confira `docker compose ps`.
- **Vite abre outra porta (ex.: 5174)** — a 5173 está ocupada; a chamada à API
  continua funcionando pelo proxy, mas o acesso é pela porta exibida no
  terminal.

## API

| Método | Rota                           | Descrição                                |
| ------ | ------------------------------ | ---------------------------------------- |
| GET    | `/api/customers`               | Lista clientes                           |
| GET    | `/api/customers/search?term=`  | Busca por CPF ou nome (tela de vendas)   |
| POST   | `/api/customers`               | Cadastra (409 se CPF duplicado)          |
| PUT    | `/api/customers/{id}`          | Atualiza                                 |
| DELETE | `/api/customers/{id}`          | Remove (restrito se houver vendas)       |
| GET    | `/api/products`                | Lista produtos                           |
| POST   | `/api/products`                | Cadastra (409 se nome duplicado)         |
| PUT    | `/api/products/{id}`           | Atualiza                                 |
| DELETE | `/api/products/{id}`           | Remove                                   |
| GET    | `/api/sales`                   | Lista vendas com itens                   |
| POST   | `/api/sales`                   | Registra venda (calcula total no server) |

Erros de validação retornam `400`/`409` com corpo `{ "message": "…" }` e
mensagens claras em pt-BR.

## Validações e regras de negócio

- CPF: 11 dígitos, dígito verificador válido, único no banco
- Cliente: nome obrigatório; e-mail validado quando informado
- Produto: nome único; valor unitário ≥ 0
- Venda: data sugere o dia atual; pelo menos 1 item; quantidade > 0; preço
  unitário capturado do produto no momento do lançamento; total calculado no
  backend (o frontend exibe, mas não define)
- Conflitos de unicidade do banco são traduzidos em mensagens amigáveis
  (ex.: "CPF já cadastrado")

## Checklist do escopo

- [x] Cadastro de clientes (CPF único, nome obrigatório, nascimento, e-mail,
      telefone)
- [x] Cadastro de produtos (nome único, valor unitário)
- [x] Tela de vendas com seleção de cliente por CPF/nome, múltiplos itens e
      totalizadores
- [x] React + .NET 8 + PostgreSQL via EF Core
- [x] PKs, FKs, índices, NOT NULL, normalização
- [x] Script SQL completo de criação do banco
- [x] Clean Architecture / SOLID (diferencial)
- [x] Swagger/OpenAPI (diferencial)
- [x] Testes automatizados nas regras de negócio (diferencial)
- [x] Documentação das decisões — este README (diferencial)
