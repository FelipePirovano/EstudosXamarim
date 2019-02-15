using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Xml.Serialization;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using EstudosXamarim;
using EstudosXamarim.Model;
using MeusPedidos.Adapter;
using MeusPedidos.Model;
using Newtonsoft.Json.Linq;
using Org.Json;

namespace MeusPedidos
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {

        RecyclerView listaReciclavelProdutos;
        RecyclerView.LayoutManager layoutManager;
        ProdutoAdapter produtoAdapter;
        List<Produto> listaProdutos;
        List<Produto> listaProdutosSelecionados;
        List<Produto> listaProdutosFiltrada;
        Android.Support.V7.Widget.Toolbar toolbar;
        Android.Widget.ProgressBar progressBar;
        Button botaoConfirmarPedido;
        LinearLayout ll_confirmar_pedido;
        LinearLayout ll_resumo_itens_selecionados;
        ScrollView ic_confirmar_pedido;
        LinearLayout ll_recebe_produtos;
        LayoutInflater inflater;
        TextView tv_quantidade_itens_selecionados;
        TextView tv_valor_total_itens_selecionados;
        float valorTotalPedidos;
        int quantidadeTotalProdutos;
        int SOLICITANDO_PEDIDO = 0;
        int CONFIRMAAR_PEDIDO = 1;
        int ESTADO_TELA_ATIVO = 0;
        List<Promocao> listaPromocoes;
        List<Categoria> listaCategorias;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            toolbar.SetTitle(Resource.String.titulo_toolbar);
            SetSupportActionBar(toolbar);

            listaReciclavelProdutos = FindViewById<RecyclerView>(Resource.Id.listaReciclavelProdutos);
            progressBar = FindViewById<Android.Widget.ProgressBar>(Resource.Id.progressBar);

            botaoConfirmarPedido = FindViewById<Button>(Resource.Id.bt_confirmar_pedido);
            ll_confirmar_pedido = FindViewById<LinearLayout>(Resource.Id.ll_confirmar_pedido);
            ic_confirmar_pedido = FindViewById<ScrollView>(Resource.Id.ic_confirmar_pedido);
            ll_recebe_produtos = FindViewById<LinearLayout>(Resource.Id.ll_recebe_produtos);
            tv_quantidade_itens_selecionados = FindViewById<TextView>(Resource.Id.tv_quantidade_itens_selecionados);
            tv_valor_total_itens_selecionados = FindViewById<TextView>(Resource.Id.tv_valor_total_itens_selecionados);
            ll_resumo_itens_selecionados = FindViewById<LinearLayout>(Resource.Id.ll_resumo_itens_selecionados);

            inflater = (LayoutInflater)this.GetSystemService(Context.LayoutInflaterService);

            consumirDadosListarPromocoes();

            botaoConfirmarPedido.Click += delegate {

                acaoBotaoConformeTela();

            };

       

        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);

            consumirDadosFiltroPorCategoria(menu);

            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            
            if (id == Android.Resource.Id.Home)
            {
                visibilidadeTela(SOLICITANDO_PEDIDO);
                rezetarLayoutConfirmarPedido();

            }else if(id == Resource.Id.todas_categorias)
            {
                produtoAdapter = new ProdutoAdapter(listaProdutos);
                listaReciclavelProdutos.SetAdapter(produtoAdapter);
            }
            else
            {
                filtrarItensListaConformeCategoria(id);
            }

            return base.OnOptionsItemSelected(item);
        }

        private void InicializarRecyclerView()
        {

            layoutManager = new LinearLayoutManager(this);
            listaReciclavelProdutos.SetLayoutManager(layoutManager);

            produtoAdapter = new ProdutoAdapter(listaProdutos);
            listaReciclavelProdutos.SetAdapter(produtoAdapter);

            listaProdutosSelecionados = new List<Produto>();

            if (listaProdutos.Count > 1)
            {

                progressBar.Visibility = ViewStates.Gone;
                listaReciclavelProdutos.Visibility = ViewStates.Visible;

            }
        }

        private void acaoBotaoConformeTela()
        {

            if (ESTADO_TELA_ATIVO == SOLICITANDO_PEDIDO)
            {
                if(listaProdutosSelecionados.Count > 0)
                {
                    gerarLayoutConfirmarPedido();
                }
            }
            else if (ESTADO_TELA_ATIVO == CONFIRMAAR_PEDIDO)
            {
                finalizarPedido();
            }

        }

        private void finalizarPedido()
        {

            Android.App.AlertDialog.Builder alert = new Android.App.AlertDialog.Builder(this);
            alert.SetTitle("PEDIDO FINALIZADO!");
            alert.SetMessage("Seu pedido está finalizado e pronto para ser confirmado, está tudo certo ?");
 
            alert.SetPositiveButton("Tudo certo!", (senderAlert, args) => {
                
                valorTotalPedidos = 0;
                quantidadeTotalProdutos = 0;
                listaProdutos.Clear();
                listaProdutosSelecionados.Clear();
                rezetarLayoutConfirmarPedido();
                visibilidadeTela(SOLICITANDO_PEDIDO);
                consumirDadosListarProdutos();

            });

            alert.SetNegativeButton("Ainda não terminei.", (senderAlert, args) => {

                alert.Dispose();

            });

            Dialog dialog = alert.Create();
            dialog.Show();
            
        }

        private void gerarLayoutConfirmarPedido()
        {

            for (int i = 0; i < listaProdutosSelecionados.Count; i++)
            {

                Produto produto = listaProdutosSelecionados[i];

                View itemListaProdutoSelecionado = inflater.Inflate(Resource.Layout.itemListaProdutoSelecionado, null);
                TextView tv_item_nome_selecionado = itemListaProdutoSelecionado.FindViewById<TextView>(Resource.Id.tv_item_nome_selecionado);
                TextView tv_item_quantidade_selecionado = itemListaProdutoSelecionado.FindViewById<TextView>(Resource.Id.tv_item_quantidade_selecionado);
                TextView tv_item_valor_selecionado = itemListaProdutoSelecionado.FindViewById<TextView>(Resource.Id.tv_item_valor_selecionado);
                ImageView iv_item_imagem_selecionado = itemListaProdutoSelecionado.FindViewById<ImageView>(Resource.Id.iv_item_imagem_selecionado);
                LinearLayout ic_desconto = itemListaProdutoSelecionado.FindViewById<LinearLayout>(Resource.Id.ic_desconto);
                TextView tv_desconto = itemListaProdutoSelecionado.FindViewById<TextView>(Resource.Id.tv_desconto);
                
                iv_item_imagem_selecionado.SetImageBitmap(GetImageBitmapFromUrl(produto.urlPhoto));
                tv_item_nome_selecionado.Text = produto.nome;
                tv_item_quantidade_selecionado.Text = produto.quantidade + " UN";
                tv_item_valor_selecionado.Text = "R$ " + produto.preco;
                
                if (produto.desconto > 0)
                {
                    ic_desconto.Visibility = ViewStates.Visible;
                    tv_desconto.Text = produto.desconto + "%";
                }

                ll_recebe_produtos.AddView(itemListaProdutoSelecionado);

            }

            visibilidadeTela(CONFIRMAAR_PEDIDO);

        }
        
        private Bitmap GetImageBitmapFromUrl(string url)
        {
            Bitmap imageBitmap = null;

            using (var webClient = new WebClient())
            {
                var imageBytes = webClient.DownloadData(url);
                if (imageBytes != null && imageBytes.Length > 0)
                {
                    imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                }
            }

            return imageBitmap;
        }
        
        public void adicionarProduto(int id, string nome, string descricao, string urlPhoto, int desconto , float preco, int categoria, int quantidade)
        {

            Produto produto = new Produto();
            produto.id = id;
            produto.nome = nome;
            produto.descricao = descricao;
            produto.urlPhoto = urlPhoto;
            produto.preco = preco;
            produto.desconto = desconto;
            produto.categoria = categoria;
            produto.quantidade = quantidade;

            for (int i = 0; i < listaProdutosSelecionados.Count; i++)
            {
                Produto validarProduto = listaProdutosSelecionados[i];

                if (validarProduto.id == produto.id)
                {
                    listaProdutosSelecionados.Remove(validarProduto);
                }
            }

            listaProdutosSelecionados.Add(produto);
            quantidadeTotalProdutos++;

            calcularValorTotal();
            atualizarTextoBotaoConfirmar();

        }

        public void removerProduto(int id, string nome, string descricao, string urlPhoto, int desconto, float preco, int categoria, int quantidade)
        {

            Produto produto = new Produto();
            produto.id = id;
            produto.nome = nome;
            produto.descricao = descricao;
            produto.urlPhoto = urlPhoto;
            produto.preco = preco;
            produto.desconto = desconto;
            produto.categoria = categoria;
            produto.quantidade = quantidade;
            
            for (int i = 0; i < listaProdutosSelecionados.Count; i++)
            {
                Produto validarProduto = listaProdutosSelecionados[i];

                if (validarProduto.id == produto.id)
                {
                    if(validarProduto.quantidade == 1)
                    {
                        listaProdutosSelecionados.Remove(validarProduto);
                        validarProduto.quantidade -= 1;
                    }
                    else
                    {
                        validarProduto.quantidade -= 1;
                    }
                }
            }

            quantidadeTotalProdutos--;
            calcularValorTotal();
            atualizarTextoBotaoConfirmar();
    
        }

        public void calcularValorTotal()
        {

            valorTotalPedidos = 0;

            for (int i = 0; i < listaProdutosSelecionados.Count; i++)
            {

                Produto produto = listaProdutosSelecionados[i];
                valorTotalPedidos += produto.preco * produto.quantidade;
              
            }

        }

        private void visibilidadeTela(int ESTADO_TELA)
        {
            listaReciclavelProdutos.Visibility = ViewStates.Gone;
            ic_confirmar_pedido.Visibility = ViewStates.Gone;
            ll_resumo_itens_selecionados.Visibility = ViewStates.Gone;


            if (ESTADO_TELA == SOLICITANDO_PEDIDO)
            {
                listaReciclavelProdutos.Visibility = ViewStates.Visible;
                toolbar.SetTitle(Resource.String.titulo_toolbar);
                ESTADO_TELA_ATIVO = ESTADO_TELA;
                SupportActionBar.SetDisplayHomeAsUpEnabled(false);
                atualizarTextoBotaoConfirmar();
            }
            else if (ESTADO_TELA == CONFIRMAAR_PEDIDO)
            {
                ic_confirmar_pedido.Visibility = ViewStates.Visible;
                ll_resumo_itens_selecionados.Visibility = ViewStates.Visible;
                toolbar.SetTitle(Resource.String.titulo_toolbar_confirmar);
                ESTADO_TELA_ATIVO = ESTADO_TELA;
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                botaoConfirmarPedido.Text = "FINALIZAR A COMPRA";
                tv_quantidade_itens_selecionados.Text = quantidadeTotalProdutos + " UN";
                tv_valor_total_itens_selecionados.Text = "R$" + valorTotalPedidos;

            };

        }

        private void atualizarTextoBotaoConfirmar()
        {

            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ",";
            botaoConfirmarPedido.Text = "COMPRAR > R$" +  valorTotalPedidos.ToString("##.##", nfi);

        }

        private void rezetarLayoutConfirmarPedido()
        {
            ll_recebe_produtos.RemoveAllViews();
        }

        private void filtrarItensListaConformeCategoria(int id)
        {

            if(listaProdutosFiltrada.Count > 0)
            {
                listaProdutosFiltrada.Clear();
            }
            
            for(int i = 0; i < listaProdutos.Count; i++)
            {

                Produto produto = listaProdutos[i];
                
                if (listaCategorias[id - 1].id  == produto.categoria)
                {

                    listaProdutosFiltrada.Add(produto);

                }
            }

            ProdutoAdapter adapterFiltrado = new ProdutoAdapter(listaProdutosFiltrada);
            listaReciclavelProdutos.SetAdapter(adapterFiltrado);
            
        }

        private void consumirDadosListarProdutos()
        {

            var request = HttpWebRequest.Create(string.Format(@"https://pastebin.com/raw/eVqp7pfX"));
            request.ContentType = "application/json";
            request.Method = "GET";

            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                if (response.StatusCode != HttpStatusCode.OK)
                    Console.Out.WriteLine("Problemas no retorno da chamada listarProdutos", response.StatusCode);
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    var content = reader.ReadToEnd();

                    if (string.IsNullOrWhiteSpace(content))
                    {
                        Console.Out.WriteLine("Json vazio no retorno da chamada ListarProdutos");
                    }
                    else
                    {
                        JArray jsonArrayProdutos = JArray.Parse(content);
                        listaProdutos = new List<Produto>();

                        for (int i = 0; i < jsonArrayProdutos.Count; i++)
                        {

                            var produto = new Produto();
                            var obj = jsonArrayProdutos[i];

                            produto.id = obj["id"].Value<int>();
                            produto.nome = obj["name"].Value<string>();
                            produto.descricao = obj["description"].Value<string>();
                            produto.urlPhoto = obj["photo"].Value<string>();
                            produto.preco = obj["price"].Value<float>();
                            produto.precoFixo = obj["price"].Value<float>();

                            if (obj["category_id"].Type != JTokenType.Null)
                            {
                                produto.categoria = obj["category_id"].Value<int>();
                            }

                            for (int position = 0; position < listaPromocoes.Count; position++)
                            {

                                Promocao promocao = listaPromocoes[position];

                                if (produto.categoria == promocao.categoria)
                                {
                                    produto.promocao = promocao;
                                }
                            }

                            listaProdutos.Add(produto);

                        }

                        InicializarRecyclerView();

                    }
                }
            }
        }

        private void consumirDadosListarPromocoes()
        {

            var request = HttpWebRequest.Create(string.Format(@"https://pastebin.com/raw/R9cJFBtG"));
            request.ContentType = "application/json";
            request.Method = "GET";

            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                if (response.StatusCode != HttpStatusCode.OK)
                    Console.Out.WriteLine("Problemas no retorno da chamada listarPromocoes", response.StatusCode);
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    var content = reader.ReadToEnd();

                    if (string.IsNullOrWhiteSpace(content))
                    {
                        Console.Out.WriteLine("Json vazio no retorno da chamada listarPromocoes");
                    }
                    else
                    {

                        JArray jsonArrayPromocoes = JArray.Parse(content);
                        listaPromocoes = new List<Promocao>();

                        for (int i = 0; i < jsonArrayPromocoes.Count; i++)
                        {
                            var promocao = new Promocao();
                            var obj = jsonArrayPromocoes[i];

                            promocao.nome = obj["name"].Value<string>();
                            promocao.categoria = obj["category_id"].Value<int>();
                            JArray arrayPoliticas = obj["policies"].Value<JArray>();

                            List<Politicas> listaPoliticas = new List<Politicas>();

                            for (int position = 0; position < arrayPoliticas.Count; position++)
                            {

                                Politicas politica = new Politicas();

                                var objPromocoes = arrayPoliticas[position];

                                politica.quantidadeMinima = objPromocoes["min"].Value<int>();
                                politica.desconto = objPromocoes["discount"].Value<int>();

                                listaPoliticas.Add(politica);

                            }

                            promocao.politicas = listaPoliticas;
                            listaPromocoes.Add(promocao);

                        }

                        consumirDadosListarProdutos();

                    }
                }
            }
        }

        private void consumirDadosFiltroPorCategoria(IMenu menu)
        {

            var request = HttpWebRequest.Create(string.Format(@"http://pastebin.com/raw/YNR2rsWe"));
            request.ContentType = "application/json";
            request.Method = "GET";

            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                if (response.StatusCode != HttpStatusCode.OK)
                    Console.Out.WriteLine("Problemas no retorno da chamada FiltroPorCategoria", response.StatusCode);
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    var content = reader.ReadToEnd();

                    if (string.IsNullOrWhiteSpace(content))
                    {
                        Console.Out.WriteLine("Json vazio no retorno da chamada FiltroPorCategoria");
                    }
                    else
                    {

                        JArray jsonArrayCategoria = JArray.Parse(content);
                        listaCategorias = new List<Categoria>();
                        listaProdutosFiltrada = new List<Produto>();

                        for (int i = 0; i < jsonArrayCategoria.Count; i++)
                        {
                            var obj = jsonArrayCategoria[i];
                            Categoria categoria = new Categoria();

                            categoria.nome = obj["name"].Value<string>();
                            categoria.id = obj["id"].Value<int>();

                            listaCategorias.Add(categoria);

                            menu.Add(0, categoria.id, 0, categoria.nome);

                        }
                    }
                }
            }
        }

    }
}