using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Xml.Serialization;
using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using EstudosXamarim;
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
        Android.Widget.ProgressBar progressBar;
        Button botaoConfirmarPedido;
        int valorTotalPedidos;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            toolbar.SetTitle(Resource.String.titulo_toolbar);
            SetSupportActionBar(toolbar);

            listaReciclavelProdutos = FindViewById<RecyclerView>(Resource.Id.listaReciclavelProdutos);
            progressBar = FindViewById<Android.Widget.ProgressBar>(Resource.Id.progressBar);

            botaoConfirmarPedido = FindViewById<Button>(Resource.Id.bt_confirmar_pedido);

            consumirDadosListarProdutos();
            atualizarTextoBotaoConfirmar();

        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        public void InicializarRecyclerView()
        {

            layoutManager = new LinearLayoutManager(this);
            listaReciclavelProdutos.SetLayoutManager(layoutManager);

            produtoAdapter = new ProdutoAdapter(listaProdutos);
            listaReciclavelProdutos.SetAdapter(produtoAdapter);
            
            if (listaProdutos.Count > 1) {

                progressBar.Visibility = ViewStates.Gone;
                listaReciclavelProdutos.Visibility = ViewStates.Visible;

            }      
        }

        public void verificarProdutosSelecionados()
        {
         
            /*for (int i = 0; i < listaProdutos.Count; i++)
            {

                Produto produto = listaProdutos[i];

                if (produto.quantidade > 0)
                {
                    listaProdutosSelecionados.Add(produto);
                }
            }*/
        }

        public void verificarValoresProdutosSelecionados()
        {

            for (int i = 0; i < listaProdutosSelecionados.Count; i++)
            {

                Produto produto = listaProdutos[i];

                if (produto.quantidade > 0)
                {
                    int valorProduto = produto.preco * produto.quantidade;
                    valorTotalPedidos += valorProduto;
                }
            }
        }

        public void atualizarTextoBotaoConfirmar()
        {

         //   botaoConfirmarPedido.Text = valorTotalPedidos.ToString();

        }

        public void consumirDadosListarProdutos()
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
                        JArray jsonArrayProdutos  = JArray.Parse(content);
                        listaProdutos = new List<Produto>();

                        for (int i = 0; i < jsonArrayProdutos.Count; i++)
                        {

                            var produto = new Produto();
                            var obj  = jsonArrayProdutos[i];

                            produto.id = obj["id"].Value<int>();
                            produto.nome = obj["name"].Value<string>();
                            produto.descricao = obj["description"].Value<string>();
                            produto.urlPhoto = obj["photo"].Value<string>();
                            produto.preco = obj["price"].Value<int>();
                           // produto.categoria = obj["category_id"].Value<int>();
                            
                            listaProdutos.Add(produto);
                            
                        }

                        InicializarRecyclerView();

                    }
                }
            }
        }
    }
}