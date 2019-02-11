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

namespace EstudosXamarim.Model
{
    class Promocao
    {

        public string nome { get; set; }
        public int categoria { get; set; }
        public List<Politicas> politicas  { get; set; }
      
    }
    class Politicas
    {
        public int quantidadeMinima { get; set; }
        public int desconto { get; set; }
    }
}