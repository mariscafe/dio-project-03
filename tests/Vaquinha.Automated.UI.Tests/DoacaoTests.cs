using FluentAssertions;
using OpenQA.Selenium;
using System;
using Vaquinha.Tests.Common.Fixtures;
using Xunit;

namespace Vaquinha.AutomatedUITests
{
	public class DoacaoTests : IDisposable, IClassFixture<DoacaoFixture>, 
                                            IClassFixture<EnderecoFixture>, 
                                            IClassFixture<CartaoCreditoFixture>
	{
		private readonly DriverFactory _driverFactory = new DriverFactory();
		private IWebDriver _driver;

		private readonly DoacaoFixture _doacaoFixture;
		private readonly EnderecoFixture _enderecoFixture;
		private readonly CartaoCreditoFixture _cartaoCreditoFixture;

		public DoacaoTests(DoacaoFixture doacaoFixture, EnderecoFixture enderecoFixture, CartaoCreditoFixture cartaoCreditoFixture)
        {
            _doacaoFixture = doacaoFixture;
            _enderecoFixture = enderecoFixture;
            _cartaoCreditoFixture = cartaoCreditoFixture;
        }
		public void Dispose()
		{
			_driverFactory.Close();
		}

		[Fact]
		public void DoacaoUI_AcessoTelaHome()
		{
			// Arrange
			_driverFactory.NavigateToUrl("https://localhost:44317/");
			_driver = _driverFactory.GetWebDriver();

			// Act
			IWebElement webElement = _driver.FindElement(By.ClassName("vaquinha-logo"));

			// Assert
			webElement.Displayed.Should().BeTrue(because:"logo exibido");
		}
		[Fact]
		public void DoacaoUI_AcessoTelaDoacao()
		{
			//Arrange
			_driverFactory.NavigateToUrl("https://localhost:44317/");
			_driver = _driverFactory.GetWebDriver();

			//Act
			IWebElement webElement = _driver.FindElement(By.ClassName("btn-blue"));
			webElement.Click();

			//Assert
			_driver.Url.Should().Contain("/Doacoes/Create");
		}

		[Fact]
		public void DoacaoUI_PreenchimentoIncorretoDoacao()
		{
			//Arrange
			_driverFactory.NavigateToUrl("https://localhost:44317/Doacoes/Create");
			_driver = _driverFactory.GetWebDriver();

			//Act
			IWebElement webElement = _driver.FindElement(By.ClassName("btn-blue"));
			webElement.Click();

			//Assert
			_driver.FindElement(By.ClassName("noty_layout")).Displayed.Should().BeTrue(because: "Campos obrigatórios não preenchidos");
		}

		[Fact]
		public void DoacaoUI_PreenchimentoCorretoDoacao()
		{
			//Arrange
			var doacao = _doacaoFixture.DoacaoValida();
			doacao.AdicionarEnderecoCobranca(_enderecoFixture.EnderecoValido());
			doacao.AdicionarFormaPagamento(_cartaoCreditoFixture.CartaoCreditoValido());

			_driverFactory.NavigateToUrl("https://localhost:44317/Doacoes/Create");
			_driver = _driverFactory.GetWebDriver();

			//Act    
			IWebElement campoValor = _driver.FindElement(By.Id("valor"));
			campoValor.SendKeys(doacao.Valor.ToString());

			IWebElement campoNome = _driver.FindElement(By.Id("DadosPessoais_Nome"));
			campoNome.SendKeys(doacao.DadosPessoais.Nome);

			IWebElement campoEmail = _driver.FindElement(By.Id("DadosPessoais_Email"));
			campoEmail.SendKeys(doacao.DadosPessoais.Email);

			IWebElement campoEndereco = _driver.FindElement(By.Id("EnderecoCobranca_TextoEndereco"));
			campoEndereco.SendKeys(doacao.EnderecoCobranca.TextoEndereco);

			IWebElement campoNumero = _driver.FindElement(By.Id("EnderecoCobranca_Numero"));
			campoNumero.SendKeys(doacao.EnderecoCobranca.Numero);

			IWebElement campoCidade = _driver.FindElement(By.Id("EnderecoCobranca_Cidade"));
			campoCidade.SendKeys(doacao.EnderecoCobranca.Cidade);

			IWebElement campoCEP = _driver.FindElement(By.Id("cep"));
			campoCEP.SendKeys(doacao.EnderecoCobranca.CEP);

			IWebElement campoTelefone = _driver.FindElement(By.Id("telefone"));
			campoTelefone.SendKeys(doacao.EnderecoCobranca.Telefone);

			IWebElement campoTitular = _driver.FindElement(By.Id("FormaPagamento_NomeTitular"));
			campoTitular.SendKeys(doacao.FormaPagamento.NomeTitular);

			IWebElement campoCartao = _driver.FindElement(By.Id("cardNumber"));
			campoCartao.SendKeys(doacao.FormaPagamento.NumeroCartaoCredito);

			IWebElement campoValidade = _driver.FindElement(By.Id("validade"));
			campoValidade.SendKeys(doacao.FormaPagamento.Validade);

			IWebElement campoCVV = _driver.FindElement(By.Id("cvv"));
			campoCVV.SendKeys(doacao.FormaPagamento.CVV);

			IWebElement webElement = _driver.FindElement(By.ClassName("btn-blue"));
			webElement.Click();

			//Assert
			_driver.Title.Should().Contain("Home", because: "Formulário preenchido corretamente");
		}
	}
}