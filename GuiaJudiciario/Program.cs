using HtmlAgilityPack;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

bool gravar = false; // alterar para true para salvar arquivo csv
string txt_gravar = "";

const string url_1 = "http://www8.tjmg.jus.br/servicos/gj/guia/primeira_instancia/consulta.do?codigoComposto=MG_";
string url_NroComarca = "1";
const string url_2 = "&paginaFlag=forum&paginaForum=1&paginaJuizado=1&opcConsulta=6&pagina=";
string url_pagina = "1";

string url_Comarca = url_1 + url_NroComarca + url_2 + url_pagina;
var doc = new HtmlWeb().Load(url_Comarca);

ArrayList list_comarca = new ArrayList();
string[,] comarca = new string[1, 2];

int i = 0, j = 0;
foreach (HtmlNode table in doc.DocumentNode.SelectNodes("//table"))   // listagem das comarcas e população da list_comarca
{
    i++; j = 0;
    if (i == 4)
    {
        foreach (HtmlNode nodes in table.SelectNodes("//option"))
        {
            j++;
            if (j > 1 && j < 1622) // comarcas conhecidas até o momento
            {
                Console.WriteLine("[" + j + "] " + nodes.Attributes[0].Value.ToString().Replace("MG_", "") + " - " + nodes.InnerText);
                comarca[0, 0] = nodes.Attributes[0].Value.ToString().Replace("MG_", "");
                comarca[0, 1] = nodes.InnerText;

                if (j == 38 || j == 148) // apenas para teste
                {
                    list_comarca.Add(comarca);
                }
                comarca = new string[1, 2];
            }
        }
    }
}
doc = null;
Regex regex = new Regex(@"\s+");
/// Carregando as paginas da comarca mediante listagem 
foreach (string[,] municipio in list_comarca)
{
    int nro = Int32.Parse(municipio[0, 0]);
    url_Comarca = url_1 + nro + url_2 + "1";
    doc = new HtmlWeb().Load(url_Comarca);
    guiajudiciario(doc, nro, municipio[0, 1]);
    doc = null;
}

if (gravar)
{
    gravar_(txt_gravar, "Guia Judiciario - " + DateTime.Now.ToString("yyyy MM dd HH mm ss ff"));
}

void gravar_(string text, string nameFile)
{
    try
    {
        string filefolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop).ToString() + @"\TJMG\";
        if (!Directory.Exists(filefolder)) { Directory.CreateDirectory(filefolder); }
        StreamWriter sw = new StreamWriter(filefolder + nameFile + ".csv", false, Encoding.UTF8);
        Console.WriteLine("\n\n\t\t\t --->" + filefolder + nameFile + ".csv \t <------");
        string texto = HttpUtility.HtmlDecode(text.Trim());
        foreach (var t in texto)
        {
            sw.Write(t);
        }
        sw.Close();
    }
    catch (Exception e)
    {
        Console.WriteLine("Exception: " + e.Message);
    }
    finally
    {
        Console.WriteLine("Executing finally block.");
    }
}
void guiajudiciario(HtmlDocument document, int nro_Comarca, string nome_Comarca)
{
    string txt_Comarca = string.Empty;
    Regex regex = new Regex(@"\s+");
    i = 0;
    Console.WriteLine("\n\t ------- >" + nro_Comarca + " - " + nome_Comarca + "< ----------");
    string txtHtml = regex.Replace(document.DocumentNode.OuterHtml, " ");
    txtHtml = txtHtml.Replace("&nbsp;", "");

    // Verificação
    bool t1 = true, t2 = true, t3 = true, t4 = true, t5 = true;

    string busc = "Atenção: Sua consulta não retornou nenhum resultado";
    string busc_1 = "Resultado da Busca pelo F&oacute;rum do Munic&iacute;pio";
    int ini_trecho1 = txtHtml.IndexOf(busc_1, 1);
    if (ini_trecho1 == -1) { t1 = false; }

    string busc_2 = "Resultado da Busca pelo(s) Juizado(s) da Comarca";
    int ini_trecho2 = txtHtml.IndexOf(busc_2, 1);
    if (ini_trecho2 == -1) { t2 = false; }

    string busc_3 = "Resultado da Busca pelos Munic&iacute;pios e Distritos Integrantes";
    int ini_trecho3 = txtHtml.IndexOf(busc_3, 1);
    if (ini_trecho3 == -1) { t3 = false; }

    string busc_4 = "Resultado da Busca por Substitui&ccedil;&atilde;o de Comarcas Vagas";
    int ini_trecho4 = txtHtml.IndexOf(busc_4, 1);
    if (ini_trecho4 == -1) { t4 = false; }

    string busc_5 = "Resultado da Busca por Servi&ccedil;os Notariais e Registros";
    int ini_trecho5 = txtHtml.IndexOf(busc_5, 1);
    if (ini_trecho5 == -1) { t5 = false; }

    string paginador = "página 1 de";
    int tem_pag = txtHtml.IndexOf(paginador);

    int iniV = txtHtml.IndexOf(busc, 1);

    if (iniV > 0)
    {
        if (iniV > ini_trecho1 && iniV < ini_trecho2 && t1) { t1 = false; }
        iniV = txtHtml.IndexOf(busc, ini_trecho2);
        if (iniV > ini_trecho2 && iniV < ini_trecho3 && t2) { t2 = false; }
        iniV = txtHtml.IndexOf(busc, ini_trecho3);
        if (iniV > ini_trecho3 && iniV < ini_trecho4 && t3) { t3 = false; }
        iniV = txtHtml.IndexOf(busc, ini_trecho4);
        if (iniV > ini_trecho4 && iniV < ini_trecho5 && t4) { t4 = false; }
        iniV = txtHtml.IndexOf(busc, ini_trecho5);
        if (iniV > ini_trecho5 && t5) { t5 = false; }
    }

    int ini1, ini2, fim, tamanho;
    string trecho1, trecho1_cab, trecho2, trecho3, trecho4, trecho5;


    t3 = false; // desnecessario "Resultado da Busca pelos Munic&iacute;pios e Distritos Integrantes"
    t4 = false; // desnecessario  "Resultado da Busca por Substitui&ccedil;&atilde;o de Comarcas Vagas";
    t5 = false; // trabalhando


    if (t1)
    {
        //txt_gravar += "\n";

        /// cabeçalho        
        ini1 = ini_trecho1;
        ini2 = txtHtml.IndexOf("<table", ini1);
        fim = txtHtml.IndexOf("<!--------------------->", ini2);
        tamanho = fim - ini2;
        trecho1_cab = txtHtml.Substring(ini2, tamanho) + "</table>";

        doc = new HtmlDocument();
        doc.LoadHtml(trecho1_cab.Replace("&nbsp;", ""));

        string col1 = string.Empty,
            col2 = string.Empty,
            col3 = string.Empty,
            col4 = string.Empty,
            col5 = string.Empty,
            col6 = string.Empty,
            col7 = string.Empty,
            col8 = string.Empty,
            col9 = string.Empty,
            col10 = string.Empty,
            col11 = string.Empty,
            col12 = string.Empty,
            col13 = string.Empty,
            col14 = string.Empty;

        foreach (HtmlNode table in doc.DocumentNode.SelectNodes("table"))
        {
            i = 0;
            foreach (var row in table.SelectNodes("tr"))
            {
                i++;
                j = 0;
                foreach (var cell in row.SelectNodes("td"))
                {
                    j++;
                    if (cell.InnerText != "")
                    {
                        switch (j)
                        {
                            case 1:
                                switch (i)
                                {
                                    case 2:
                                        col1 = cell.InnerText.Trim();
                                        txt_Comarca = cell.InnerText.Trim();
                                        break;
                                    case 3:
                                        col2 = cell.InnerText;
                                        break;
                                    case 4:
                                        col4 += cell.InnerText;
                                        break;
                                    case 5:
                                        col4 += cell.InnerText;
                                        break;
                                    case 6:
                                        col4 += cell.InnerText;
                                        break;
                                }
                                break;
                            case 2:
                                switch (i)
                                {
                                    case 3:
                                        col3 = cell.InnerText;
                                        break;
                                    case 4:
                                        col7 = cell.InnerText;
                                        break;
                                }
                                break;
                            case 3:
                                switch (i)
                                {
                                    case 2:
                                        col9 = cell.InnerText;
                                        break;
                                }
                                break;
                        }
                    }
                }
            }
        }

        col1 = col1.Trim();

        Console.WriteLine("\n" + col1 + "; " + col2 + "; ;" + col3 + "; " + col4 + "; ; ; " + col7 + "; ; ; ; ; ;" + col9 + "\n");
        txt_gravar += "\n" + col1 + ";" + ";" + col2 + ";" + col3 + ";" + col4 + ";;;" + col7 + ";;;;;;" + col9 + ";";
        /// fim cabeçalho        
        //gravar_(txt_gravar1, "Trecho1 Cabeçalho - " + DateTime.Now.ToString("yyyy MM dd HH mm ss ff"));

        /// TEM PAGINAS?
        if (tem_pag == -1)
        {
            //txt_gravar += "\n";
            ini1 = fim;
            ini2 = txtHtml.IndexOf("<table", ini1);
            fim = txtHtml.IndexOf("</table>", ini2) + ("</table>").Length;
            tamanho = fim - ini2;
            trecho1 = txtHtml.Substring(ini2, tamanho);


            doc = new HtmlDocument();
            doc.LoadHtml(trecho1.Replace("&nbsp;", ""));

            string atributo = string.Empty,
                titulo_tabela = string.Empty,
                txt_linha = string.Empty,
                email = string.Empty,
                linha = string.Empty,
                email_ant = string.Empty;
            bool start = true;
            foreach (HtmlNode table in doc.DocumentNode.SelectNodes("table"))
            {
                txt_linha = "";
                foreach (var row in table.SelectNodes("tr"))
                {
                    // TR
                    atributo = row.GetAttributeValue("class", "");
                    if (row.GetAttributeValue("class", "") != string.Empty) // temos um atributo (linha ou titulo de tabela)
                    {
                        //atributo = row.GetAttributeValue("class", "");
                        if (row.GetAttributeValue("class", "") == "titulo_tabela") // Se class = titulo_tabela, temos uma nova tabela
                        {
                            start = true;
                            col3 = row.SelectSingleNode("td[1]").InnerText; // Titulo da Tabela;                            
                            email_ant = email;
                            titulo_tabela = row.SelectSingleNode("td[1]").InnerText;
                            email = row.SelectSingleNode("td[3]").InnerText;
                            linha = string.Empty;
                        }
                        else if ((row.GetAttributeValue("class", "")).StartsWith("linha"))
                        {
                            start = false;
                            if ((row.GetAttributeValue("class", "")) != linha) // Temos uma nova linha
                            {
                                linha = (row.GetAttributeValue("class", ""));
                                col4 = row.SelectSingleNode("td/b").InnerText;

                                Console.Write("\n\n\n" + col1 + "; " + col2 + "; " + col3 + "; ");
                                txt_gravar += "\n" + col1 + "; " + col2 + "; " + col3 + "; ";
                                foreach (var cell in row.SelectNodes("td"))
                                {
                                    Console.Write(HttpUtility.HtmlDecode(cell.InnerText) + "; ");
                                    txt_gravar += HttpUtility.HtmlDecode(cell.InnerText) + ";";
                                }
                            }
                            else if ((row.GetAttributeValue("class", "")) == linha)// continuamos na mesma linha
                            {
                                foreach (var cell in row.SelectNodes("td"))
                                {
                                    Console.Write(HttpUtility.HtmlDecode(cell.InnerText) + "; ");
                                    txt_gravar += HttpUtility.HtmlDecode(cell.InnerText) + ";";
                                }
                            }
                        }
                        else
                        {
                            start = false;
                        }
                    }
                    // Fim do TR
                    if (row.SelectSingleNode("td").GetAttributeValue("colspan", "") == "4")
                    {
                        if (start)
                        {
                            Console.Write("\n" + col1 + "; " + col2 + "; " + col3 + ";;;;;;;;;; ");
                            txt_gravar += "\n" + col1 + "; " + col2 + "; " + col3 + ";;;;;;;;;; ";
                            start = false;
                        }
                        Console.Write("; " + email);
                    }
                }
            }
            //Console.Write(email + ";");
            //txt_gravar += email + ";";
        }
        else
        {
            //txt_gravar += "\n";
            doc = new HtmlDocument();
            trecho1 = dadospaginados(nro_Comarca);
            doc.LoadHtml(trecho1.Replace("&nbsp;", ""));

            string atributo = string.Empty,
                titulo_tabela = string.Empty,
                txt_linha = string.Empty,
                email = string.Empty,
                linha = string.Empty,
                email_ant = string.Empty;
            bool start = true;
            foreach (HtmlNode table in doc.DocumentNode.SelectNodes("table"))
            {
                txt_linha = "";
                foreach (var row in table.SelectNodes("tr"))
                {
                    // TR
                    atributo = row.GetAttributeValue("class", "");
                    if (row.GetAttributeValue("class", "") != string.Empty) // temos um atributo (linha ou titulo de tabela)
                    {
                        if (row.GetAttributeValue("class", "") == "titulo_tabela") // Se class = titulo_tabela, temos uma nova tabela
                        {
                            start = true;
                            col3 = row.SelectSingleNode("td[1]").InnerText; // Titulo da Tabela;                            
                            email_ant = email;
                            titulo_tabela = row.SelectSingleNode("td[1]").InnerText;
                            email = row.SelectSingleNode("td[3]").InnerText;
                            linha = string.Empty;
                        }
                        else if ((row.GetAttributeValue("class", "")).StartsWith("linha"))
                        {
                            start = false;
                            if ((row.GetAttributeValue("class", "")) != linha) // Temos uma nova linha
                            {
                                linha = (row.GetAttributeValue("class", ""));
                                col4 = row.SelectSingleNode("td/b").InnerText;

                                Console.Write("\n\n\n" + col1 + "; " + col2 + "; " + col3 + "; ");
                                txt_gravar += "\n" + col1 + "; " + col2 + "; " + col3 + "; ";
                                foreach (var cell in row.SelectNodes("td"))
                                {
                                    Console.Write(HttpUtility.HtmlDecode(cell.InnerText) + "; ");
                                    txt_gravar += HttpUtility.HtmlDecode(cell.InnerText) + ";";
                                }
                            }
                            else if ((row.GetAttributeValue("class", "")) == linha)// continuamos na mesma linha
                            {
                                foreach (var cell in row.SelectNodes("td"))
                                {
                                    Console.Write(HttpUtility.HtmlDecode(cell.InnerText) + "; ");
                                    txt_gravar += HttpUtility.HtmlDecode(cell.InnerText) + ";";
                                }
                            }
                        }
                        else
                        {
                            start = false;
                        }
                    }
                    // Fim do TR
                    if (row.SelectSingleNode("td").GetAttributeValue("colspan", "") == "4")
                    {
                        if (start)
                        {
                            Console.Write("\n" + col1 + "; " + col2 + "; " + col3 + ";;;;;;;;;; ");
                            txt_gravar += "\n" + col1 + "; " + col2 + "; " + col3 + ";;;;;;;;;; ";
                            start = false;
                        }
                        Console.Write("; " + email);
                    }
                }
            }
            //Console.Write(email + ";");
            //txt_gravar += email + ";";
        }
    }
    //// FIM Trecho1
    /// TRECHO 2

    if (t2)
    {
        ini1 = txtHtml.IndexOf(busc_2, 1);
        ini2 = txtHtml.IndexOf("<table", ini1);
        fim = txtHtml.IndexOf("PAGINADOR", ini2 + 10);
        tamanho = fim - ini2;
        trecho2 = txtHtml.Substring(ini2, tamanho) + "-->";

        doc = new HtmlDocument();
        doc.LoadHtml(trecho2.Replace("&nbsp;", ""));

        txt_gravar += "\n";

        string classe = string.Empty,
            cabecalho = string.Empty,
            instalacao = string.Empty,
            titulo_tabela = string.Empty,
            tabela = string.Empty,
            email1 = string.Empty,
            linha_gravar = string.Empty,
            linha_aux = string.Empty,
            atributo = string.Empty,
            txt_linha = string.Empty,
            email = string.Empty,
            linha = string.Empty,
            email_ant = string.Empty;

        bool start_Table = false, start = true;

        foreach (HtmlNode tables in doc.DocumentNode.SelectNodes("table"))
        {
            start_Table = false;
            ////Console.Write(">: " + tables.Name + ": \t" + tables.XPath + "\n");
            foreach (var head in tables.SelectNodes("tr"))
            {
                //// Console.Write(">>: " + head.Name + ": \t" + head.XPath);
                classe = head.GetAttributeValue("class", " ");
                if (start_Table)
                {
                    foreach (var table in head.SelectNodes("td"))
                    {
                        //// Console.Write("\n>>>: " + table.Name + ": \t" + table.XPath + ": ");
                        if (table.HasChildNodes)
                        {
                            foreach (var table_ in table.SelectNodes("table"))
                            {
                                ////Console.Write("\n>>>: " + table_.Name + ": \t" + table_.XPath + ": ");
                                /// start_Titulo = false;
                                if (table_.HasAttributes)
                                {
                                    foreach (var row in table_.SelectNodes("tr"))
                                    {
                                        ////Console.Write("\n>>>: " + row.Name + ": \t" + row.XPath + ": " + row.GetAttributeValue("class", " ") + ": ");
                                        tabela = row.GetAttributeValue("class", " ");
                                        if (row.GetAttributeValue("class", "") != string.Empty && row.GetAttributeValue("class", "") != atributo) // temos um novo atributo (linha ou titulo de tabela)
                                        {
                                            atributo = row.GetAttributeValue("class", "");
                                            if (atributo == "titulo_tabela")
                                            {
                                                titulo_tabela = row.SelectSingleNode("td").InnerText + "; ";
                                                email_ant = email;
                                                email = row.SelectSingleNode("td[3]").InnerText;
                                                linha = string.Empty;
                                            }
                                            else if (atributo.StartsWith("linha"))
                                            {
                                                if (atributo != linha) // Temos uma nova linha
                                                {
                                                    linha = atributo;
                                                    linha_aux = "";
                                                    if (!start)
                                                    {

                                                        Console.Write(email_ant + ";" + instalacao + "\n\n\n");
                                                        linha_aux = email_ant + ";" + instalacao;
                                                    }
                                                    Console.Write("\n" + txt_Comarca + ";" + cabecalho + "; " + titulo_tabela);
                                                    txt_gravar += linha_aux + "\n" + txt_Comarca + ";" + cabecalho + "; " + titulo_tabela;
                                                }
                                                else { } // continuamos na mesma linha
                                            }
                                        }
                                        foreach (var cell in row.SelectNodes("td"))
                                        {
                                            if (atributo.StartsWith("linha"))
                                            {
                                                if (start) { start = false; }
                                                Console.Write(HttpUtility.HtmlDecode(cell.InnerText));
                                                //Console.Write(";");
                                                txt_gravar += HttpUtility.HtmlDecode(cell.InnerText) + ";";
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (classe == "cabecalho") ///ENCONTROU NOVO CLASS CABEÇALHO 
                {
                    cabecalho = HttpUtility.HtmlDecode(head.SelectSingleNode("td[1]").InnerText);
                    instalacao = HttpUtility.HtmlDecode(head.SelectSingleNode("td[2]").InnerText);
                    //Console.Write(" -> Cabeçalho: " + cabecalho + "; " + instalacao);
                    start_Table = true;
                }
                else { start_Table = false; }
                Console.Write("\n");
                //txt_gravar += "\n";
            }
        }
        /// FIM TRECHO 2

        /// TRECHO 3

        if (t3)
        {
            ini1 = ini_trecho3;
            ini2 = txtHtml.IndexOf("<table", ini1);
            fim = txtHtml.IndexOf("</table>", ini2) + ("</table>").Length;
            tamanho = fim - ini2;
            trecho3 = txtHtml.Substring(ini2, tamanho);
            doc = new HtmlDocument();
            doc.LoadHtml(trecho3.Replace("&nbsp;", ""));
            foreach (HtmlNode table in doc.DocumentNode.SelectNodes("table"))
            {
                i = 0;
                foreach (var row in table.SelectNodes("tr"))
                {
                    i++;
                    j = 0;
                    foreach (var cell in row.SelectNodes("td"))
                    {
                        j++;
                        Console.WriteLine("[" + i + ", " + j + "] " + cell.InnerText);
                        txt_gravar += "<br>" + "[" + i + ", " + j + "] " + cell.OuterHtml + "\n";
                        txt_gravar += "\n" + "[" + i + ", " + j + "] " + cell.OuterHtml;
                        txt_gravar += "\n" + "[" + i + ", " + j + "] " + cell.InnerText;
                    }
                }
            }
        }
        /// FIM TRECHO3

        /// TRECHO 4

        if (t4)
        {
            ini1 = ini_trecho4;
            ini2 = txtHtml.IndexOf("<table", ini1);
            fim = txtHtml.IndexOf("</table>", ini2) + ("</table>").Length;
            tamanho = fim - ini2;
            trecho4 = txtHtml.Substring(ini2, tamanho);

            doc = new HtmlDocument();
            doc.LoadHtml(trecho4.Replace("&nbsp;", ""));
            foreach (HtmlNode table in doc.DocumentNode.SelectNodes("table"))
            {
                i = 0;
                foreach (var row in table.SelectNodes("tr"))
                {
                    i++;
                    j = 0;
                    foreach (var cell in row.SelectNodes("td"))
                    {
                        j++;
                        Console.WriteLine("[" + i + ", " + j + "] " + cell.InnerText);
                        txt_gravar += "<br>" + "[" + i + ", " + j + "] " + cell.OuterHtml + "\n";
                        txt_gravar += "\n" + "[" + i + ", " + j + "] " + cell.OuterHtml;
                        txt_gravar += "\n" + "[" + i + ", " + j + "] " + cell.InnerText;
                    }
                }
            }
        }
        /// FIM TRECHO 4

        /// TRECHO 5
        if (t5)
        {
            ini1 = ini_trecho5;
            ini2 = txtHtml.IndexOf("<table", ini1);
            fim = txtHtml.IndexOf("</table>", ini2) + ("</table>").Length;
            tamanho = fim - ini2;
            trecho5 = txtHtml.Substring(ini2, tamanho);

            doc = new HtmlDocument();
            doc.LoadHtml(trecho5.Replace("&nbsp;", ""));
            foreach (HtmlNode table in doc.DocumentNode.SelectNodes("table"))
            {
                i = 0;
                foreach (var row in table.SelectNodes("tr"))
                {
                    i++;
                    j = 0;
                    foreach (var cell in row.SelectNodes("td"))
                    {
                        j++;
                        Console.WriteLine("[" + i + ", " + j + "] " + cell.InnerText);
                        txt_gravar += "<br>" + "[" + i + ", " + j + "] " + cell.OuterHtml + "\n";
                        txt_gravar += "\n" + "[" + i + ", " + j + "] " + cell.OuterHtml;
                        txt_gravar += "\n" + "[" + i + ", " + j + "] " + cell.InnerText;
                    }
                }
            }
        }
    }
    //if (gravar)
    //{
}
string dadospaginados(int nro_Comarca)
{
    Regex regex = new Regex(@"\s+");

    string dados = "";

    int pag = 1;
    int total_pag = 1;
    string url_pag = url_1 + nro_Comarca + url_2 + pag.ToString();
    var doc_pag = new HtmlWeb().Load(url_pag);

    string tag_pag = "";
    tag_pag = doc_pag.DocumentNode.ChildNodes[1].ChildNodes[5].SelectNodes("table")[2].ChildNodes[1].InnerText;

    total_pag = Int32.Parse(tag_pag.Replace("página 1 de ", ""));

    for (int i = 1; i <= total_pag; i++)
    {
        url_pag = url_1 + nro_Comarca + url_2 + i.ToString();
        doc_pag = new HtmlWeb().Load(url_pag);

        string txtHtml = regex.Replace(doc_pag.DocumentNode.OuterHtml, " ");
        string text_busca = "Orgãos e Setores";
        string text_busca2 = "<table ";
        string text_busca3 = "</table>";

        int ini1, ini2, fim;

        ini1 = txtHtml.IndexOf(text_busca);
        ini2 = txtHtml.IndexOf(text_busca2, ini1);
        fim = txtHtml.IndexOf(text_busca3, ini2);

        txtHtml = txtHtml.Substring(ini2, fim - ini2 + text_busca3.Length);

        dados += txtHtml + "\n";
        doc_pag = new HtmlDocument();
        //doc_pag.LoadHtml(txtHtml);
    }

    return dados;
}
