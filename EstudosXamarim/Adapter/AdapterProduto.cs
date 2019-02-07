using System;

using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using MeusPedidos.Model;
using System.Collections.Generic;
using EstudosXamarim;

namespace MeusPedidos.Adapter
{
    class ProdutoAdapter : RecyclerView.Adapter
    {
        public event EventHandler<Adapter1ClickEventArgs> ItemClick;
        public event EventHandler<Adapter1ClickEventArgs> ItemLongClick;
        List<Produto> items;

        public ProdutoAdapter(List<Produto> data)
        {
            items = data;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {

            View itemView = null;
            //var id = Resource.Layout.itemListaProduto ;
            var id = Resource.Layout.itemListaProduto;
            itemView = LayoutInflater.From(parent.Context).Inflate(id, parent, false);
            var vh = new Adapter1ViewHolder(itemView, OnClick, OnLongClick);
            return vh;
        }


        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            var item = items[position];
            var holder = viewHolder as Adapter1ViewHolder;

            holder.nome.Text = items[position].nome;
            holder.descricao.Text = items[position].descricao;

        }

        public override int ItemCount => items.Count;

        void OnClick(Adapter1ClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(Adapter1ClickEventArgs args) => ItemLongClick?.Invoke(this, args);

    }

    public class Adapter1ViewHolder : RecyclerView.ViewHolder
    {
        public TextView nome { get; set; }
        public TextView descricao { get; set; }

        public Adapter1ViewHolder(View itemView, Action<Adapter1ClickEventArgs> clickListener,
                            Action<Adapter1ClickEventArgs> longClickListener) : base(itemView)
        {
          
            nome = itemView.FindViewById<TextView>(Resource.Id.tv_item_nome);
            descricao = itemView.FindViewById<TextView>(Resource.Id.tv_item_descricao);

            itemView.Click += (sender, e) => clickListener(new Adapter1ClickEventArgs { View = itemView, Position = AdapterPosition });
            itemView.LongClick += (sender, e) => longClickListener(new Adapter1ClickEventArgs { View = itemView, Position = AdapterPosition });
        }
    }

    public class Adapter1ClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}