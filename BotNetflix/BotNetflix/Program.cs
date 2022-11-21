using System;
using OpenQA.Selenium;
using prmToolkit.Selenium;
using prmToolkit.Selenium.Enum;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;

namespace BotNetflix
{
    class Program
    {
        static void Main(string[] args)
        {
            string localDriver = System.AppDomain.CurrentDomain.BaseDirectory.ToString();
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--start-maximized");
            options.AddArgument("--disable-notifications");
            IWebDriver webDriver = new ChromeDriver(localDriver + "/driver", options);
            var insclass = new Program();
            




            webDriver.Navigate().GoToUrl("https://www.netflix.com/pt/");
            
            System.Threading.Thread.Sleep(500);
            insclass.Wait(webDriver, By.XPath("/html/body/div[1]/div/div/div/div/div/div[1]/div[1]/div/div[1]/button[1]"));

            webDriver.FindElement(By.XPath("/html/body/div[1]/div/div/div/div/div/div[1]/div[1]/div/div[1]/button[1]")).Click();

            insclass.Wait(webDriver, By.ClassName("authLinks"));
            webDriver.FindElement(By.ClassName("authLinks")).Click();


            insclass.Login(webDriver, "tiago-coelho@sapo.pt", "pass", "code");
            System.Threading.Thread.Sleep(2000);
            //webDriver.Navigate().GoToUrl("https://www.netflix.com/watch/80145116?trackId=14277283&tctx=-97%2C-97%2C%2C%2C%2C%2C%2C%2C80095697");
            //insclass.Wait(webDriver, By.ClassName("icon-logoUpdate"));
            //webDriver.FindElement(By.ClassName("icon-logoUpdate")).Click();
            //insclass.Pesquisar(webDriver, "6548");
            //insclass.SelecCont(webDriver, "1", "2");
            
            //insclass.IgnorarIntro(webDriver);
            System.Threading.Thread.Sleep(7000);
            //insclass.Saltar10(webDriver);
            //System.Threading.Thread.Sleep(2000);
            //insclass.DiminuirSom(webDriver);
            //System.Threading.Thread.Sleep(2000);
            //insclass.DiminuirSom(webDriver);
            //System.Threading.Thread.Sleep(2000);
            //insclass.AumentarSom(webDriver);
            //System.Threading.Thread.Sleep(2000);
            //insclass.AumentarSom(webDriver);
            //System.Threading.Thread.Sleep(2000);
            //insclass.Mutar(webDriver);
            //System.Threading.Thread.Sleep(7000);
            //insclass.Desmutar(webDriver);

            insclass.ShowTopSeries(webDriver);
            insclass.SelectTop(webDriver, "9");
            insclass.ShowTopFilmes(webDriver);
            insclass.SelectTop(webDriver, "9");


            //PESQUISAR FILMES/SÉRIES:
            //ACAO - "10673"
            //COMEDIA - "10375"
            //DRAMA - "11714"
            //Romance - "26156"
            //Terror - "83059"
            //insclass.Pesquisar(webDriver, "10673");
            //insclass.SelecCont(webDriver, "2", "3");
            //System.Threading.Thread.Sleep(10000);

            //PESQUISAR IDIOMAS:
            //INGLÊS - "107548"
            //FRANCES - "100378"
            //ESPANHOL - "100396"
            //PORTUGUES - "107465"
            //insclass.ShowIdioma(webDriver, "100396");
            //System.Threading.Thread.Sleep(2000);
            //insclass.SelectContIdioma(webDriver, "2", "3");

            //insclass.ShowNotFinished(webDriver);
            //System.Threading.Thread.Sleep(2000);
            //insclass.SelectNotFinished(webDriver, "2", "2");






        }
        public void Mutar(IWebDriver webDriver)
        {
            System.Threading.Thread.Sleep(1000);
            Actions action = new Actions(webDriver);
            action.SendKeys(Keys.ArrowDown);
            action.SendKeys(Keys.ArrowDown);
            action.SendKeys(Keys.ArrowDown);
            action.SendKeys(Keys.ArrowDown);
            action.SendKeys(Keys.ArrowDown);
            action.SendKeys(Keys.ArrowDown);
            action.SendKeys(Keys.ArrowDown);
            action.SendKeys(Keys.ArrowDown);
            action.SendKeys(Keys.ArrowDown);
            action.SendKeys(Keys.ArrowDown);
            action.SendKeys(Keys.ArrowDown);
            action.Build().Perform();
        }
        public void Desmutar(IWebDriver webDriver)
        {
            System.Threading.Thread.Sleep(1000);
            Actions action = new Actions(webDriver);
            action.SendKeys(Keys.ArrowUp);
            action.SendKeys(Keys.ArrowUp);
            action.SendKeys(Keys.ArrowUp);
            action.SendKeys(Keys.ArrowUp);
            action.SendKeys(Keys.ArrowUp);
            action.SendKeys(Keys.ArrowUp);
            action.SendKeys(Keys.ArrowUp);
            action.SendKeys(Keys.ArrowUp);
            action.SendKeys(Keys.ArrowUp);
            action.SendKeys(Keys.ArrowUp);
            action.SendKeys(Keys.ArrowUp);
            action.Build().Perform();
        }
        public void AumentarSom(IWebDriver webDriver)
        {
            System.Threading.Thread.Sleep(1000);
            Actions action = new Actions(webDriver);
            action.SendKeys(Keys.ArrowUp);
            action.SendKeys(Keys.ArrowUp);
            action.SendKeys(Keys.ArrowUp);
            action.SendKeys(Keys.ArrowUp);
            action.SendKeys(Keys.ArrowUp);
            
            action.Build().Perform();
        }
        public void DiminuirSom(IWebDriver webDriver)
        {
            System.Threading.Thread.Sleep(1000);
            Actions action = new Actions(webDriver);
            action.SendKeys(Keys.ArrowDown);
            action.SendKeys(Keys.ArrowDown);
            action.SendKeys(Keys.ArrowDown);
            action.SendKeys(Keys.ArrowDown);
            action.SendKeys(Keys.ArrowDown);
            
            action.Build().Perform();
        }
        public void Saltar10(IWebDriver webDriver)
        {
            System.Threading.Thread.Sleep(1000);
            Actions action = new Actions(webDriver);
            action.SendKeys(Keys.ArrowRight);
            action.Build().Perform();
        }
        public void Retroceder10(IWebDriver webDriver)
        {
            System.Threading.Thread.Sleep(1000);
            Actions action = new Actions(webDriver);
            action.SendKeys(Keys.ArrowLeft);
            action.Build().Perform();
        }
        public void IgnorarIntro(IWebDriver webDriver)
        {
            Actions action = new Actions(webDriver);
            Wait(webDriver, By.ClassName("ltr-1212o1j"));
            action.MoveToElement(webDriver.FindElement(By.ClassName("ltr-1212o1j"))).Perform();
            Wait(webDriver, By.ClassName("ltr-1d02up3"));
            webDriver.FindElement(By.ClassName("ltr-1d02up3")).Click();
        }
        public void SairCont_reproduzir(IWebDriver webDriver)
        {
            System.Threading.Thread.Sleep(1000);
            Actions action = new Actions(webDriver);
            Wait(webDriver, By.ClassName("ltr-1212o1j"));
            action.MoveToElement(webDriver.FindElement(By.ClassName("ltr-1212o1j"))).Perform();
            Wait(webDriver, By.ClassName("ltr-14ph5iy"));
            webDriver.FindElement(By.ClassName("ltr-14ph5iy")).Click();
        }
        public void ReproduzirCont(IWebDriver webDriver)
        {
            System.Threading.Thread.Sleep(1000);
            Wait(webDriver, By.ClassName("ltr-1jtux27"));
            webDriver.FindElement(By.ClassName("ltr-1jtux27")).Click();
        }
        public void FecharCont(IWebDriver webDriver)
        {
            Wait(webDriver, By.ClassName("previewModal-close"));
            webDriver.FindElement(By.ClassName("previewModal-close")).Click();
        }

        public void Login(IWebDriver webDriver, String mail, String pass, String code)
        {
            
            Wait(webDriver, By.Name("userLoginId"));
            IWebElement login_mail = webDriver.FindElement(By.Name("userLoginId"));
            login_mail.SendKeys(mail);

            Wait(webDriver, By.Name("password"));
            IWebElement login_pass = webDriver.FindElement(By.Name("password"));
            login_pass.SendKeys(pass);
            System.Threading.Thread.Sleep(1000);

            Wait(webDriver, By.XPath("/html/body/div[1]/div/div[3]/div/div/div[1]/form/button"));
            webDriver.FindElement(By.XPath("/html/body/div[1]/div/div[3]/div/div/div[1]/form/button")).Click();
            Wait(webDriver, By.XPath("/html/body/div[1]/div/div/div[1]/div[1]/div[2]/div/div/ul/li[1]/div/a/div/div"));
            webDriver.FindElement(By.XPath("/html/body/div[1]/div/div/div[1]/div[1]/div[2]/div/div/ul/li[1]/div/a/div/div")).Click();

            foreach (char c in code)
            {

                webDriver.SendKeys(By.ClassName("focus-visible"), c.ToString());
            }


        }

        public void Pesquisar(IWebDriver webDriver, String text)
        {
            Wait(webDriver, By.ClassName("searchBox"));
            webDriver.FindElement(By.ClassName("searchBox")).Click();
            Wait(webDriver, By.Id("searchInput"));
            IWebElement pesq = webDriver.FindElement(By.Id("searchInput"));
            pesq.SendKeys(text);

        }

        public void SelecCont(IWebDriver webDriver, String linha, String cont)
        {
            Wait(webDriver, By.XPath("/html/body/div[1]/div/div/div[1]/div[1]/div[2]/div/div/div[2]/div[2]/div/div/div[" + linha + "]/div/div/div/div/div/div[" + cont + "]/div/div"));
            webDriver.FindElement(By.XPath("/html/body/div[1]/div/div/div[1]/div[1]/div[2]/div/div/div[2]/div[2]/div/div/div[" + linha + "]/div/div/div/div/div/div[" + cont + "]/div/div")).Click();
        }

        public void ShowTopSeries(IWebDriver webDriver)
        {
            webDriver.Navigate().GoToUrl("https://www.netflix.com/browse/genre/83");
            IJavaScriptExecutor jse = (IJavaScriptExecutor)webDriver;
            Wait(webDriver, By.ClassName("forward-leaning"));
            IWebElement elementTop = webDriver.FindElement(By.ClassName("forward-leaning"));
            jse.ExecuteScript("arguments[0].scrollIntoView(true)", elementTop);
        }
        public void ShowTopFilmes(IWebDriver webDriver)
        {
            webDriver.Navigate().GoToUrl("https://www.netflix.com/browse/genre/34399");
            IJavaScriptExecutor jse = (IJavaScriptExecutor)webDriver;
            Wait(webDriver, By.ClassName("forward-leaning"));
            IWebElement elementTop = webDriver.FindElement(By.ClassName("forward-leaning"));
            jse.ExecuteScript("arguments[0].scrollIntoView(true)", elementTop);
        }

        public void SelectTop(IWebDriver webDriver, String pos)
        {
            int posi = int.Parse(pos) - 1;
            if (posi>5)
            {
                Wait(webDriver, By.ClassName("handleNext"));
                System.Threading.Thread.Sleep(1000);
                webDriver.FindElement(By.ClassName("handleNext")).Click();
            }
            System.Threading.Thread.Sleep(2000);
            Wait(webDriver, By.Id("title-card-1-" + posi));
            webDriver.FindElement(By.Id("title-card-1-" + posi)).Click();
        }
        public void ShowIdioma(IWebDriver webDriver, String code)
        {
            webDriver.Navigate().GoToUrl("https://www.netflix.com/browse/original-audio/" + code);
        }
        public void ShowNotFinished(IWebDriver webDriver)
        {
            webDriver.Navigate().GoToUrl("https://www.netflix.com/browse/m/continue-watching");
        }


        public void SelectContIdioma(IWebDriver webDriver, String linha, String pos)
        {
            int posi = int.Parse(pos) - 1;
            int linhai = int.Parse(linha) - 1;
            pos = posi.ToString();
            linha = linhai.ToString();
            System.Threading.Thread.Sleep(2000);
            Wait(webDriver, By.Id("title-card-" + linha + "-" + pos));
            webDriver.FindElement(By.Id("title-card-"+linha+"-" + pos)).Click();
        }

        public void SelectNotFinished(IWebDriver webDriver, String linha, String pos)
        {
            int posi = int.Parse(pos) - 1;
            int linhai = int.Parse(linha) - 1;
            pos = posi.ToString();
            linha = linhai.ToString();
            Wait(webDriver, By.ClassName("focus-trap-wrapper"));
            IWebElement div = webDriver.FindElement(By.ClassName("focus-trap-wrapper"));

            div.FindElement(By.Id("title-card-" + linha + "-" + pos)).Click();

        }
        public void Wait(IWebDriver webDriver, By by)
        {
            try
            {
                WebDriverWait w = new WebDriverWait(webDriver, TimeSpan.FromSeconds(20));
                w.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(by));
                System.Threading.Thread.Sleep(500);
            }
            catch {
                //TO DO
                // Quando elemento nao encontrado retornar informacao ao utilizador que não possível e para função onde wait foi chamado
                Console.WriteLine("Not found");
            }

        }
        


    }

}