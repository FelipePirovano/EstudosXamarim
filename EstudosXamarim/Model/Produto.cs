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
    }
}