using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml.Linq;
using mmisharp;
using Newtonsoft.Json;
using OpenQA.Selenium;
using prmToolkit.Selenium;
using prmToolkit.Selenium.Enum;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;


namespace AppGui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MmiCommunication mmiC;

        //  new 16 april 2020
        private MmiCommunication mmiSender;
        private LifeCycleEvents lce;
        private MmiCommunication mmic;
       
        public static class WebDriver
        {
            public static IWebDriver webDriver;
            public static void InitializeDriver()
            {
                string localDriver = System.AppDomain.CurrentDomain.BaseDirectory.ToString();
                ChromeOptions options = new ChromeOptions();
                options.AddArgument("--start-maximized");
                options.AddArgument("--disable-notifications");
                WebDriver.webDriver = new ChromeDriver(localDriver + "/driver", options);
                
            }

            public static IWebDriver GetWebDriver()
            {
                return WebDriver.webDriver;
            }
        }

        public static class AppContext
        {
            public static String context;
            public static Boolean nextPage;

            public static void ChangeContext(String c) 
            {
                AppContext.context = c;
            }

            public static String GetContext()
            {
                return AppContext.context;
            }
        }
        public MainWindow()
        {
            InitializeComponent();
            WebDriver.InitializeDriver();
            AppContext.context = "";
            AppContext.nextPage = false;

            mmiC = new MmiCommunication("localhost", 8000, "User1", "GUI");
            mmiC.Message += MmiC_Message;
            mmiC.Start();

            // NEW 16 april 2020
            //init LifeCycleEvents..
            lce = new LifeCycleEvents("APP", "TTS", "User1", "na", "command"); // LifeCycleEvents(string source, string target, string id, string medium, string mode
            // MmiCommunication(string IMhost, int portIM, string UserOD, string thisModalityName)
            mmic = new MmiCommunication("localhost", 8000, "User1", "GUI");


        }

        private void MmiC_Message(object sender, MmiEventArgs e)
        {
            Console.WriteLine(e.Message);
            var doc = XDocument.Parse(e.Message);
            var com = doc.Descendants("command").FirstOrDefault().Value;
            dynamic json = JsonConvert.DeserializeObject(com);

        
            switch ((string)json.recognized[0].ToString())
            {
                case "OPEN":
                    Console.WriteLine("SELENIUM ABRIR NETFLIX");
                    
                    WebDriver.webDriver.Navigate().GoToUrl("https://www.netflix.com/pt/");
                    System.Threading.Thread.Sleep(500);
                    Wait(WebDriver.webDriver, By.XPath("/html/body/div[1]/div/div/div/div/div/div[1]/div[1]/div/div[1]/button[1]"));

                    WebDriver.webDriver.FindElement(By.XPath("/html/body/div[1]/div/div/div/div/div/div[1]/div[1]/div/div[1]/button[1]")).Click();

                    Wait(WebDriver.webDriver, By.ClassName("authLinks"));
                    WebDriver.webDriver.FindElement(By.ClassName("authLinks")).Click();
                    break;
                case "PUT":
                    Console.WriteLine("SELENIUM COLOCAR DADOS");
                    
                    Login(WebDriver.webDriver, "tiago-coelho@sapo.pt", "pass", "code");

                    break;

                case "SHOWTOP":
                    
                    Console.WriteLine("SELENIUM SHOW TOP");
                    if (json.recognized[1].ToString() == "FILM")
                    {
                        AppContext.nextPage = false;
                        AppContext.context = "TopFilmes";
                        ShowTopFilmes(WebDriver.webDriver);
                    }
                    else if (json.recognized[1].ToString() == "SERIES")
                    {
                        AppContext.nextPage = false;
                        AppContext.context = "TopSeries";
                        ShowTopSeries(WebDriver.webDriver);
                    }
                    break;

                case "TOP":
                    if (json.recognized[2].ToString() == "FILM")
                    {
                        SelectTop(WebDriver.webDriver, json.recognized[1].ToString());
                    }
                    else if(json.recognized[2].ToString() == "SERIES")
                    {
                        SelectTopSeries(WebDriver.webDriver, json.recognized[1].ToString());
                    }
                    break;
                
                case "NFinished":
                    AppContext.context = "NFinished";
                    ShowNotFinished(WebDriver.webDriver);
                    Console.WriteLine("SELENIUM MOSTRAR CONTEÙDO NOT FINISHED");
                    break;

                case "FILM":
                    try
                    {
                        AppContext.context = "SEARCH";
                        // pesquisar o tipo de filme

                        searchTypeOfFilm(json.recognized[1].ToString());
                        
                    } catch
                    {
                        Console.WriteLine("SELENIUM PESQUISAR TODOS OS FILMES");
                    }
                    break;

                case "SERIES":
                   try
                    {
                        AppContext.context = "SEARCH";
                        // pesquisar o tipo de filme
                        searchTypeOfSeries(json.recognized[1].ToString());
                    } catch
                    {
                        Console.WriteLine("SELENIUM PESQUISAR TODAS AS SERIES");
                    }
                    break;
                case "SELECT":
                    Console.WriteLine(AppContext.context);
                    if (AppContext.context == "SEARCH")
                    {
                        SelecCont(WebDriver.webDriver, json.recognized[2].ToString(), json.recognized[1].ToString());
                    }
                    else if (AppContext.context == "IDIOMA")
                    {
                        SelectContIdioma(WebDriver.webDriver, json.recognized[2].ToString(), json.recognized[1].ToString());
                    }
                    else if (AppContext.context == "NFinished")
                    {
                        SelectNotFinished(WebDriver.webDriver, json.recognized[2].ToString(), json.recognized[1].ToString());

                    }
                    else if (AppContext.context == "TopFilmes" || AppContext.context == "TopSeries")
                    {
                        if (json.recognized[2].ToString() == "FILM")
                        {
                            SelectTop(WebDriver.webDriver, json.recognized[2].ToString());
                        }
                        else if (json.recognized[2].ToString() == "SERIES")
                        {
                            SelectTopSeries(WebDriver.webDriver, json.recognized[2].ToString());
                        }
                    }
                    break;
                case "NEXTPAGETOP":
                    if (AppContext.context == "TopFilmes")
                    {
                        AppContext.nextPage = !AppContext.nextPage;
                        NextPageFilmes(WebDriver.webDriver);
                    }
                    else if (AppContext.context == "TopSeries")
                    {
                        AppContext.nextPage = !AppContext.nextPage;
                        NextPageSeries(WebDriver.webDriver);
                    }
                    break;
                case "START":
                    ReproduzirCont(WebDriver.webDriver);
                    break;
                case "PLAY":
                    Play(WebDriver.webDriver);
                    break;
                case "PAUSE":
                    Pause(WebDriver.webDriver);
                    break;
                case "UP":
                    AumentarSom(WebDriver.webDriver);
                    break;
                case "DOWN":
                    DiminuirSom(WebDriver.webDriver);
                    break;
                case "MUTE":
                    Mutar(WebDriver.webDriver);
                    break;
                case "UNMUTE":
                    Desmutar(WebDriver.webDriver);
                    break;
                case "JUMPINTRO":
                    IgnorarIntro(WebDriver.webDriver);
                    break;
                case "JUMP10":
                    Saltar10(WebDriver.webDriver);
                    break;
                case "JUMPBACK10":
                    Retroceder10(WebDriver.webDriver);
                    break;
                case "BACK":
                    SairCont_reproduzir(WebDriver.webDriver);
                    break;
                case "CANCEL":
                    FecharCont(WebDriver.webDriver);
                    break;
                case "RELOAD":
                    RefreshPage(WebDriver.webDriver);
                    break;
                case "BACKPAGE":
                    GoBackPage(WebDriver.webDriver);
                    break;
                case "MOREINFO":
                    break;


            }


            //  new 16 april 2020
            mmic.Send(lce.NewContextRequest());

            string json2 = "{ \"synthesize\": [\"";


            Console.WriteLine((string)json.recognized[0].ToString());
            Console.WriteLine(json.recognized);

            try
            {
                json2 += (string)json.recognized[0].ToString() + " ";
                json2 += (string)json.recognized[1].ToString() + " ";
                json2 += (string)json.recognized[2].ToString() + " DONE.";
            }
            catch
            {
                json2 += "DONE.";

            }
            json2 += "\"] }";

            Console.WriteLine(json2);
            /*
             foreach (var resultSemantic in e.Result.Semantics)
            {
                json += "\"" + resultSemantic.Value.Value + "\", ";
            }
            json = json.Substring(0, json.Length - 2);
            json += "] }";
            */
            var exNot = lce.ExtensionNotification(0 + "", 0 + "", 1, json2);
            mmic.Send(exNot);


        }
        private void Wait(IWebDriver webDriver, By by)
        {
            try
            {
                WebDriverWait w = new WebDriverWait(webDriver, TimeSpan.FromSeconds(20));
                w.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(by));
                System.Threading.Thread.Sleep(500);
            }
            catch
            {
                //TO DO
                // Quando elemento nao encontrado retornar informacao ao utilizador que não possível e para função onde wait foi chamado
                Console.WriteLine("Not found");
            }

        }
        private void GoBackPage(IWebDriver webDriver)
        {
            webDriver.Navigate().Refresh();
            Actions action = new Actions(webDriver);

            action.SendKeys(Keys.Alt);
            action.SendKeys(Keys.ArrowLeft);

            action.Build().Perform();
        }
        private void RefreshPage(IWebDriver webDriver)
        {

            webDriver.Navigate().Refresh();
        }
        private void FecharCont(IWebDriver webDriver)
        {
            Wait(webDriver, By.ClassName("focus-trap-wrapper"));
            IWebElement div = webDriver.FindElement(By.ClassName("focus-trap-wrapper"));

            div.FindElement(By.ClassName("previewModal-close")).Click();
        }
        private void SairCont_reproduzir(IWebDriver webDriver)
        {
            System.Threading.Thread.Sleep(1000);
            Actions action = new Actions(webDriver);
            Wait(webDriver, By.ClassName("ltr-1212o1j"));
            action.MoveToElement(webDriver.FindElement(By.ClassName("ltr-1212o1j"))).Perform();
            Wait(webDriver, By.ClassName("ltr-14ph5iy"));
            webDriver.FindElement(By.ClassName("ltr-14ph5iy")).Click();
        }
        private void Play(IWebDriver webDriver)
        {

            Actions action = new Actions(webDriver);
            action.SendKeys(Keys.Space);

            action.Build().Perform();
        }
        private void Saltar10(IWebDriver webDriver)
        {
            System.Threading.Thread.Sleep(1000);
            Actions action = new Actions(webDriver);
            action.SendKeys(Keys.ArrowRight);
            action.Build().Perform();
        }
        private void Retroceder10(IWebDriver webDriver)
        {
            System.Threading.Thread.Sleep(1000);
            Actions action = new Actions(webDriver);
            action.SendKeys(Keys.ArrowLeft);
            action.Build().Perform();
        }
        private void IgnorarIntro(IWebDriver webDriver)
        {
            Actions action = new Actions(webDriver);
            Wait(webDriver, By.ClassName("ltr-1212o1j"));
            action.MoveToElement(webDriver.FindElement(By.ClassName("ltr-1212o1j"))).Perform();
            Wait(webDriver, By.ClassName("ltr-1d02up3"));
            webDriver.FindElement(By.ClassName("ltr-1d02up3")).Click();
        }
        private void Pause(IWebDriver webDriver)
        {

            Actions action = new Actions(webDriver);
            action.SendKeys(Keys.Space);

            action.Build().Perform();
        }
        private void Mutar(IWebDriver webDriver)
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
        private void Desmutar(IWebDriver webDriver)
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
        private void AumentarSom(IWebDriver webDriver)
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
        private void DiminuirSom(IWebDriver webDriver)
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
        private void Login(IWebDriver webDriver, String mail, String pass, String code)
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
        private void ShowTopFilmes(IWebDriver webDriver)
        {
            webDriver.Navigate().GoToUrl("https://www.netflix.com/browse/genre/34399");
            IJavaScriptExecutor jse = (IJavaScriptExecutor)webDriver;
            Wait(webDriver, By.ClassName("forward-leaning"));
            IWebElement elementTop = webDriver.FindElement(By.ClassName("forward-leaning"));
            jse.ExecuteScript("arguments[0].scrollIntoView(true)", elementTop);
        }
        private void ShowNotFinished(IWebDriver webDriver)
        {
            webDriver.Navigate().GoToUrl("https://www.netflix.com/browse/m/continue-watching");
        }
        private void ShowTopSeries(IWebDriver webDriver)
        {
            webDriver.Navigate().GoToUrl("https://www.netflix.com/browse/genre/83");
            IJavaScriptExecutor jse = (IJavaScriptExecutor)webDriver;
            Wait(webDriver, By.ClassName("forward-leaning"));
            IWebElement elementTop = webDriver.FindElement(By.ClassName("forward-leaning"));
            jse.ExecuteScript("arguments[0].scrollIntoView(true)", elementTop);
        }

        private void SelectTopSeries(IWebDriver webDriver, String pos)
        {
            int posi = int.Parse(pos) - 1;
            if ((posi > 5 && AppContext.nextPage == false) || (posi <= 5 && AppContext.nextPage == true))
            {
                Wait(webDriver, By.ClassName("handleNext"));
                System.Threading.Thread.Sleep(1000);
                webDriver.FindElement(By.ClassName("handleNext")).Click();
            }
            System.Threading.Thread.Sleep(2000);
            Wait(webDriver, By.Id("title-card-1-" + posi));
            webDriver.FindElement(By.Id("title-card-1-" + posi)).Click();
        }
        private void NextPageFilmes(IWebDriver webDriver)
        {
            Wait(webDriver, By.ClassName("handleNext"));
            System.Threading.Thread.Sleep(1000);
            webDriver.FindElement(By.ClassName("handleNext")).Click();
        }
        private void NextPageSeries(IWebDriver webDriver)
        {
            Wait(webDriver, By.Id("row-4"));
            IWebElement div = webDriver.FindElement(By.Id("row-4"));
            div.FindElement(By.ClassName("handleNext")).Click();
        }
        private void SelectTop(IWebDriver webDriver, String pos)
        {
            int posi = int.Parse(pos) - 1;
            if ((posi > 5 && AppContext.nextPage == false) || (posi <= 5 && AppContext.nextPage == true))
            {
                Wait(webDriver, By.ClassName("handleNext"));
                System.Threading.Thread.Sleep(1000);
                webDriver.FindElement(By.ClassName("handleNext")).Click();
            }
            System.Threading.Thread.Sleep(2000);
            Wait(webDriver, By.Id("title-card-1-" + posi));
            webDriver.FindElement(By.Id("title-card-1-" + posi)).Click();
        }
        private void SelectContIdioma(IWebDriver webDriver, String linha, String pos)
        {
            int posi = int.Parse(pos) - 1;
            int linhai = int.Parse(linha) - 1;
            pos = posi.ToString();
            linha = linhai.ToString();
            System.Threading.Thread.Sleep(2000);
            Wait(webDriver, By.Id("title-card-" + linha + "-" + pos));
            webDriver.FindElement(By.Id("title-card-" + linha + "-" + pos)).Click();
        }
        private void SelectNotFinished(IWebDriver webDriver, String linha, String pos)
        {
            int posi = int.Parse(pos) - 1;
            int linhai = int.Parse(linha) - 1;
            pos = posi.ToString();
            linha = linhai.ToString();
            Wait(webDriver, By.ClassName("focus-trap-wrapper"));
            IWebElement div = webDriver.FindElement(By.ClassName("focus-trap-wrapper"));

            div.FindElement(By.Id("title-card-" + linha + "-" + pos)).Click();

        }
        private void SelecCont(IWebDriver webDriver, String linha, String cont)
        {
            Wait(webDriver, By.XPath("/html/body/div[1]/div/div/div[1]/div[1]/div[2]/div/div/div[2]/div[2]/div/div/div[" + linha + "]/div/div/div/div/div/div[" + cont + "]/div/div"));
            webDriver.FindElement(By.XPath("/html/body/div[1]/div/div/div[1]/div[1]/div[2]/div/div/div[2]/div[2]/div/div/div[" + linha + "]/div/div/div/div/div/div[" + cont + "]/div/div")).Click();
        }
        private void ReproduzirCont(IWebDriver webDriver)
        {
            System.Threading.Thread.Sleep(1000);
            Wait(webDriver, By.ClassName("focus-trap-wrapper"));
            IWebElement div = webDriver.FindElement(By.ClassName("focus-trap-wrapper"));

            div.FindElement(By.ClassName("ltr-1jtux27")).Click();
        }
        private void Pesquisar(IWebDriver webDriver, String text)
        {
            Wait(webDriver, By.ClassName("searchBox"));
            webDriver.FindElement(By.ClassName("searchBox")).Click();
            Wait(webDriver, By.Id("searchInput"));
            IWebElement pesq = webDriver.FindElement(By.Id("searchInput"));
            pesq.SendKeys(text);

        }
        private void ShowIdioma(IWebDriver webDriver, String code)
        {
            webDriver.Navigate().GoToUrl("https://www.netflix.com/browse/original-audio/" + code);
        }
        private void searchTypeOfFilm(string json)
        {
            switch (json)
            {
                case "ACTION":
                    Console.WriteLine("SELENIUM PESQUISAR FILMES DE AÇÃO");
                    Pesquisar(WebDriver.webDriver, "10673");
                    break;
                case "COMEDY":
                    Console.WriteLine("SELENIUM PESQUISAR FILMES DE COMÉDIA");
                    Pesquisar(WebDriver.webDriver, "10375");
                    break;
                case "DRAMA":
                    Console.WriteLine("SELENIUM PESQUISAR FILMES DE DRAMA");
                    Pesquisar(WebDriver.webDriver, "11714");
                    break;
                case "NOVEL":
                    Console.WriteLine("SELENIUM PESQUISAR FILMES DE ROMANCE");
                    Pesquisar(WebDriver.webDriver, "26156");
                    break;
                case "TERROR":
                    Console.WriteLine("SELENIUM PESQUISAR FILMES DE TERROR");
                    Pesquisar(WebDriver.webDriver, "83059");
                    break;
                case "CHRISTMAS":
                    Console.WriteLine("SELENIUM PESQUISAR FILMES DE TERROR");
                    Pesquisar(WebDriver.webDriver, "81346420");
                    AppContext.context = "IDIOMA";
                    break;
                case "PT":
                    ShowIdioma(WebDriver.webDriver, "107465");
                    AppContext.context = "IDIOMA";
                    break;
                case "FR":
                    ShowIdioma(WebDriver.webDriver, "100378");
                    AppContext.context = "IDIOMA";
                    break;
                case "ES":
                    ShowIdioma(WebDriver.webDriver, "100396");
                    AppContext.context = "IDIOMA";
                    break;
                case "EN":
                    ShowIdioma(WebDriver.webDriver, "107548");
                    AppContext.context = "IDIOMA";
                    break;
            }
        }

        private void searchTypeOfSeries(string json)
        {
            switch (json)
            {
                case "ACTION":
                    Console.WriteLine("SELENIUM PESQUISAR FILMES DE AÇÃO");
                    Pesquisar(WebDriver.webDriver, "10673");
                    break;
                case "COMEDY":
                    Console.WriteLine("SELENIUM PESQUISAR FILMES DE COMÉDIA");
                    Pesquisar(WebDriver.webDriver, "10375");
                    break;
                case "DRAMA":
                    Console.WriteLine("SELENIUM PESQUISAR FILMES DE DRAMA");
                    Pesquisar(WebDriver.webDriver, "11714");
                    break;
                case "NOVEL":
                    Console.WriteLine("SELENIUM PESQUISAR FILMES DE ROMANCE");
                    Pesquisar(WebDriver.webDriver, "26156");
                    break;
                case "TERROR":
                    Console.WriteLine("SELENIUM PESQUISAR FILMES DE TERROR");
                    Pesquisar(WebDriver.webDriver, "83059");
                    break;
                case "CHRISTMAS":
                    Console.WriteLine("SELENIUM PESQUISAR FILMES DE TERROR");
                    Pesquisar(WebDriver.webDriver, "81346420");
                    break;
                case "PT":
                    ShowIdioma(WebDriver.webDriver, "107465");
                    AppContext.context = "IDIOMA";
                    break;
                case "FR":
                    ShowIdioma(WebDriver.webDriver, "100378");
                    AppContext.context = "IDIOMA";
                    break;
                case "ES":
                    ShowIdioma(WebDriver.webDriver, "100396");
                    AppContext.context = "IDIOMA";
                    break;
                case "EN":
                    ShowIdioma(WebDriver.webDriver, "107548");
                    AppContext.context = "IDIOMA";
                    break;
            }
        }
    }
}