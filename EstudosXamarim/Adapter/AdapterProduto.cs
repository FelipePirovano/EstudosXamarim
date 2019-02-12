using System;

using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using MeusPedidos.Model;
using System.Collections.Generic;
using EstudosXamarim;
using Android.Content;
using EstudosXamarim.Model;
using Android.Graphics;
using System.Net;

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
            var holder = viewHolder as Adapter1ViewHolder;

            holder.nome.Text = items[position].nome;
            holder.price.Text = "R$"+ items[position].preco;
            holder.quantidade.Text = items[position].quantidade + " UN";
            holder.image.SetImageBitmap(GetImageBitmapFromUrl(items[position].urlPhoto));

            if (items[position].desconto > 0) {
                holder.campoDesconto.Visibility = ViewStates.Visible;
            }
            else
            {
                holder.campoDesconto.Visibility = ViewStates.Invisible;
            }

            holder.desconto.Text = items[position].desconto + "%";
            
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

                Produto produto = items[posicao];
                produto.quantidade++;
                
                produto.gerarDesconto(false);
   
                NotifyDataSetChanged();

                NotifyItemChanged(posicao);
                
                ((MainActivity)context).adicionarProduto(produto.id,produto.nome,produto.descricao,produto.urlPhoto,produto.preco,produto.categoria,produto.quantidade);
                
            }
            else if(v.Id == Resource.Id.bt_remover_produto)
            {
                int posicao = (int)v.Tag;

                if (items[posicao].quantidade == 0) {
                    return;
                }
                
                Produto produto = items[posicao];
                produto.quantidade--;
   
                produto.retirarDesconto();
                
                NotifyDataSetChanged();

                ((MainActivity)context).removerProduto(produto.id, produto.nome, produto.descricao, produto.urlPhoto, produto.preco, produto.categoria, produto.quantidade);

            }
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
        
    }

    
    public class Adapter1ViewHolder : RecyclerView.ViewHolder
    {
        public TextView nome { get; set; }
        public TextView price { get; set; }
        public TextView quantidade { get; set; }
        public ImageView image { get; set; }
        public Button botaoAdicionar { get; set; }
        public Button botaoRemover { get; set; }
        public LinearLayout campoDesconto { get; set; }
        public TextView desconto { get; set; }
        //public TextView promocao { get; set; }
       

        public Adapter1ViewHolder(View itemView) : base(itemView)
        {
          
            nome = itemView.FindViewById<TextView>(Resource.Id.tv_item_nome);
            price = itemView.FindViewById<TextView>(Resource.Id.tv_item_price);
            image = itemView.FindViewById<ImageView>(Resource.Id.iv_item_image);
            botaoAdicionar = itemView.FindViewById<Button>(Resource.Id.bt_adicionar_produto);
            botaoRemover = itemView.FindViewById<Button>(Resource.Id.bt_remover_produto);
            quantidade = itemView.FindViewById<TextView>(Resource.Id.tv_quantidade_produtos);
            campoDesconto = itemView.FindViewById<LinearLayout>(Resource.Id.ic_item_desconto);
            desconto = itemView.FindViewById<TextView>(Resource.Id.tv_desconto);
            //promocao = itemView.FindViewById<TextView>(Resource.Id.tv_item_promocao);

        }
    }
}