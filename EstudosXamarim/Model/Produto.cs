using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using EstudosXamarim.Model;

namespace MeusPedidos.Model
{
    class Produto
    {
        public int id { get; set; }
        public string nome { get; set; }
        public string descricao { get; set; }
        public string urlPhoto { get; set; }
        public int preco { get; set; }
        public int categoria { get; set; }
        public int quantidade { get; set; }
        public Promocao promocao { get; set; }
        public int desconto { get; set; }
        public int precoFixo { get; set; }

        public void gerarDesconto()
        {
            try
            {
                List<Politicas> arrayPoliticas = promocao.politicas;

                for (int i = 0; i < arrayPoliticas.Count; i++)
                {

                    Politicas politicas = arrayPoliticas[i];

                    if (quantidade == politicas.quantidadeMinima)
                    {
                        desconto = politicas.desconto;
                        precoFixo = preco;

                        int valorDesconto = precoFixo * desconto / 100;
                        preco = preco - valorDesconto;

                    }
                }
            }
            catch
            {
                //controle de variável null
            }
        }

        public void retirarDesconto()
        {
            preco = precoFixo;
            gerarDesconto();
        }

    }



   

}
 