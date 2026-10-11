export interface Customer {
  id: number
  cpf: string
  name: string
  birthDate: string | null
  email: string | null
  phone: string | null
}

export interface CustomerPayload {
  cpf: string
  name: string
  birthDate: string | null
  email: string | null
  phone: string | null
}

export interface Product {
  id: number
  name: string
  unitPrice: number
}

export interface ProductPayload {
  name: string
  unitPrice: number
}

export interface SaleItem {
  productId: number
  productName: string
  quantity: number
  unitPrice: number
  itemTotal: number
}

export interface Sale {
  id: number
  customerId: number
  customerName: string
  customerCpf: string
  saleDate: string
  totalAmount: number
  items: SaleItem[]
}

export interface NewSaleItemPayload {
  productId: number
  quantity: number
}

export interface NewSalePayload {
  customerId: number
  saleDate?: string
  items: NewSaleItemPayload[]
}
