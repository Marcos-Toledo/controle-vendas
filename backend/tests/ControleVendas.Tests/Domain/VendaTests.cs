using ControleVendas.Domain.Entities;

namespace ControleVendas.Tests.Domain;

public class VendaTests
{
    [Fact]
    public void RecalcularTotal_SomaTotalDosItens()
    {
        var venda = new Venda();

        venda.Itens.Add(new ItemVenda { ProdutoId = 1, Quantidade = 2, PrecoUnitario = 10.50m });
        venda.Itens.Add(new ItemVenda { ProdutoId = 2, Quantidade = 1, PrecoUnitario = 4.00m });

        venda.RecalcularTotal();

        Assert.Equal(25.00m, venda.ValorTotal);
    }

    [Fact]
    public void RecalcularTotal_SemItens_ZeraTotal()
    {
        var venda = new Venda();

        venda.RecalcularTotal();

        Assert.Equal(0m, venda.ValorTotal);
    }
}
