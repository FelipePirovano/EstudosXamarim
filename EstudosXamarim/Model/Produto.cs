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
        public float preco { get; set; }
        public int categoria { get; set; }
        public int quantidade { get; set; }
        public Promocao promocao { get; set; }
        public int desconto { get; set; }
        public float precoFixo { get; set; }
        public bool favorito { get; set; }
        
        private int ultimaUnidadeDescontoAplicada { get; set; }
        private int ultimoDescontoAplicado { get; set; }

        public void gerarDesconto(bool retirandoProduto)
        {
            try
            {
                List<Politicas> arrayPoliticas = promocao.politicas;
                
                for (int i = 0; i < arrayPoliticas.Count; i++)
                {

                    Politicas politicas = arrayPoliticas[i];

                    if (quantidade == politicas.quantidadeMinima)
                    {
                        ultimoDescontoAplicado = desconto;
                        desconto = politicas.desconto;
                        
                        float valorDesconto = precoFixo * desconto / 100;
                        float valorDescontado = preco - valorDesconto;

                        preco = valorDescontado;

                        ultimaUnidadeDescontoAplicada = quantidade;
                        
                    }
                    else if (quantidade > 0) {
                        
                        if (quantidade == ultimaUnidadeDescontoAplicada - 1)
                        {
                            
                            if (retirandoProduto)
                            {
                                desconto = ultimoDescontoAplicado;

                                float precoAnterior = precoFixo * desconto / 100;
                                preco = precoFixo - precoAnterior;
                                return;
                            }
                            
                        }

                        float descontoRules = precoFixo * desconto / 100;
                        preco = precoFixo - descontoRules;
                        
                    }else if(quantidade == 0)
                    {
                        desconto = 0;
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
            gerarDesconto(true);

        }

    }



   

}
 