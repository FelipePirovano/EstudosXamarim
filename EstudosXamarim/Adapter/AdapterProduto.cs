using System;

using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using MeusPedidos.Model;
using System.Collections.Generic;
using EstudosXamarim;
using Android.Content;

namespace MeusPedidos.Adapter
{
    class ProdutoAdapter : RecyclerView.Adapter, View.IOnClickListener
    {
        List<Produto> items;
        Context context;


        public ProdutoAdapter(List<Produto> data)
        {
            items = data;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {

            View itemView = null;
            var id = Resource.Layout.itemListaProduto;
            itemView = LayoutInflater.From(parent.Context).Inflate(id, parent, false);
            context = parent.Context;
            var vh = new Adapter1ViewHolder(itemView);
            return vh;
        }


        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            var item = items[position];
            var holder = viewHolder as Adapter1ViewHolder;

            holder.nome.Text = items[position].nome;
            holder.price.Text = "R$"+ items[position].preco;
            holder.quantidade.Text = items[position].quantidade + " UN";
       
            holder.botaoAdicionar.SetOnClickListener(this);
            holder.botaoAdicionar.Tag = position;

            holder.botaoRemover.SetOnClickListener(this);
            holder.botaoRemover.Tag = position;
            
        }

        public override int ItemCount => items.Count;
        

        public void OnClick(View v)
        {
            if (v.Id == Resource.Id.bt_adicionar_produto) {
               
                int posicao = (int)v.Tag;
                items[posicao].quantidade ++ ;

                NotifyDataSetChanged();
                
                ((MainActivity)context).verificarProdutosSelecionados();

                System.Console.Out.WriteLine("quantidade de itens:" + items[posicao].quantidade);

            }
            else if(v.Id == Resource.Id.bt_remover_produto)
            {
                int posicao = (int)v.Tag;

                if (items[posicao].quantidade == 0) {
                    return;
                }
                
                items[posicao].quantidade-- ;

                NotifyDataSetChanged();

                ((MainActivity)context).verificarProdutosSelecionados();

                System.Console.Out.WriteLine("quantidade de itens:" + items[posicao].quantidade);
            }
        }
    }

    public class Adapter1ViewHolder : RecyclerView.ViewHolder
    {
        public TextView nome { get; set; }
        public TextView price { get; set; }
        public TextView quantidade { get; set; }
        public ImageView image { get; set; }
        public Button botaoAdicionar { get; set; }
        public Button botaoRemover { get; set; }

        public Adapter1ViewHolder(View itemView) : base(itemView)
        {
          
            nome = itemView.FindViewById<TextView>(Resource.Id.tv_item_nome);
            price = itemView.FindViewById<TextView>(Resource.Id.tv_item_price);
            image = itemView.FindViewById<ImageView>(Resource.Id.iv_item_image);
            botaoAdicionar = itemView.FindViewById<Button>(Resource.Id.bt_adicionar_produto);
            botaoRemover = itemView.FindViewById<Button>(Resource.Id.bt_remover_produto);
            quantidade = itemView.FindViewById<TextView>(Resource.Id.tv_quantidade_produtos);

        }
    }
}