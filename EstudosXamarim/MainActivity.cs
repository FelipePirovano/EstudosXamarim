using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Xml.Serialization;
using Android.App;
using Android.Content;
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
        int valorTotalPedidos;
        int quantidadeTotalProdutos;
        int SOLICITANDO_PEDIDO = 0;
        int CONFIRMAAR_PEDIDO = 1;
        int ESTADO_TELA_ATIVO = 0;
        IMenuItem menu1;
        List<Promocao> listaPromocoes;

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
            menu1 = menu.GetItem(0);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;

            if (id == Resource.Id.action_settings)
            {
                return true;
            }
            else if (id == Android.Resource.Id.Home)
            {
                visibilidadeTela(SOLICITANDO_PEDIDO);
                rezetarLayoutConfirmarPedido();
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

                gerarLayoutConfirmarPedido();

                //VALIDAÇÃO ITENS LISTA

            }
            else if (ESTADO_TELA_ATIVO == CONFIRMAAR_PEDIDO)
            {

                //EFETUAR MICRO ANIMAÇÂO DENTRO DE UM DIALOG PARA INFORMAR PEDIDO FINALIZADO

            }

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

                tv_item_nome_selecionado.Text = produto.nome;
                tv_item_quantidade_selecionado.Text = produto.quantidade + " UN";
                tv_item_valor_selecionado.Text = "R$ " + produto.preco;

                ll_recebe_produtos.AddView(itemListaProdutoSelecionado);

            }

            visibilidadeTela(CONFIRMAAR_PEDIDO);

        }

        public void adicionarProduto(int id, string nome, string descricao, string urlPhoto, int preco, int categoria, int quantidade)
        {

            Produto produto = new Produto();
            produto.id = id;
            produto.nome = nome;
            produto.descricao = descricao;
            produto.urlPhoto = urlPhoto;
            produto.preco = preco;
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

        public void removerProduto(int id, string nome, string descricao, string urlPhoto, int preco, int categoria, int quantidade)
        {

            Produto produto = new Produto();
            produto.id = id;
            produto.nome = nome;
            produto.descricao = descricao;
            produto.urlPhoto = urlPhoto;
            produto.preco = preco;
            produto.categoria = categoria;
            produto.quantidade = quantidade;


            for (int i = 0; i < listaProdutosSelecionados.Count; i++)
            {
                Produto validarProduto = listaProdutosSelecionados[i];

                if (validarProduto.id == produto.id)
                {
                    validarProduto.quantidade -= 1;
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

            if (ESTADO_TELA == SOLICITANDO_PEDIDO)
            {
                listaReciclavelProdutos.Visibility = ViewStates.Visible;
                ic_confirmar_pedido.Visibility = ViewStates.Gone;
                ll_resumo_itens_selecionados.Visibility = ViewStates.Gone;
                toolbar.SetTitle(Resource.String.titulo_toolbar);
                ESTADO_TELA_ATIVO = ESTADO_TELA;
                menu1.SetVisible(true);
                SupportActionBar.SetDisplayHomeAsUpEnabled(false);
                atualizarTextoBotaoConfirmar();
            }
            else if (ESTADO_TELA == CONFIRMAAR_PEDIDO)
            {
                listaReciclavelProdutos.Visibility = ViewStates.Gone;
                ic_confirmar_pedido.Visibility = ViewStates.Visible;
                ll_resumo_itens_selecionados.Visibility = ViewStates.Visible;
                toolbar.SetTitle(Resource.String.titulo_toolbar_confirmar);
                menu1.SetVisible(false);
                ESTADO_TELA_ATIVO = ESTADO_TELA;
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                botaoConfirmarPedido.Text = "FINALIZAR A COMPRA";
                tv_quantidade_itens_selecionados.Text = quantidadeTotalProdutos + " UN";
                tv_valor_total_itens_selecionados.Text = "R$" + valorTotalPedidos;

            };

        }

        private void atualizarTextoBotaoConfirmar()
        {

            botaoConfirmarPedido.Text = "COMPRAR > R$" + valorTotalPedidos.ToString();

        }

        private void rezetarLayoutConfirmarPedido()
        {
            ll_recebe_produtos.RemoveAllViews();
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
                            produto.preco = obj["price"].Value<int>();
                            produto.precoFixo = obj["price"].Value<int>();

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

    }
}