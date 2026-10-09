export interface Cliente {
  id: number
  cpf: string
  nome: string
  dataNascimento: string | null
  email: string | null
  telefone: string | null
}

export interface ClientePayload {
  cpf: string
  nome: string
  dataNascimento: string | null
  email: string | null
  telefone: string | null
}

export interface Produto {
  id: number
  nome: string
  valorUnitario: number
}

export interface ProdutoPayload {
  nome: string
  valorUnitario: number
}

export interface ItemVenda {
  produtoId: number
  produtoNome: string
  quantidade: number
  precoUnitario: number
  totalItem: number
}

export interface Venda {
  id: number
  clienteId: number
  clienteNome: string
  clienteCpf: string
  dataVenda: string
  valorTotal: number
  itens: ItemVenda[]
}

export interface NovaVendaItemPayload {
  produtoId: number
  quantidade: number
}

export interface NovaVendaPayload {
  clienteId: number
  dataVenda?: string
  itens: NovaVendaItemPayload[]
}
