using System.Collections.Generic;
using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using EstudosXamarim;
using MeusPedidos.Adapter;
using MeusPedidos.Model;

namespace MeusPedidos
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {

        RecyclerView listaReciclavelProdutos;
        RecyclerView.LayoutManager layoutManager;
        ProdutoAdapter produtoAdapter;
        List<Produto> listaProdutos;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            toolbar.SetTitle(Resource.String.titulo_toolbar);
            SetSupportActionBar(toolbar);

            listaReciclavelProdutos = FindViewById<RecyclerView>(Resource.Id.listaReciclavelProdutos);

            InicializarRecyclerView();

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

            listaProdutos = new List<Produto>();
            listaProdutos.Add(new Produto() { nome = "teste", descricao = "teste teste teste teste" });
            listaProdutos.Add(new Produto() { nome = "teste", descricao = "teste teste teste teste" });
            listaProdutos.Add(new Produto() { nome = "teste", descricao = "teste teste teste teste" });
            listaProdutos.Add(new Produto() { nome = "teste", descricao = "teste teste teste teste" });
            listaProdutos.Add(new Produto() { nome = "teste", descricao = "teste teste teste teste" });
            listaProdutos.Add(new Produto() { nome = "teste", descricao = "teste teste teste teste" });
            listaProdutos.Add(new Produto() { nome = "teste", descricao = "teste teste teste teste" });

            produtoAdapter = new ProdutoAdapter(listaProdutos);
            listaReciclavelProdutos.SetAdapter(produtoAdapter);

        }
    }
}