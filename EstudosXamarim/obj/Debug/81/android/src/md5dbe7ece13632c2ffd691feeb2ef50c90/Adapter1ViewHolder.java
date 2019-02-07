package md5dbe7ece13632c2ffd691feeb2ef50c90;


public class Adapter1ViewHolder
	extends android.support.v7.widget.RecyclerView.ViewHolder
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("MeusPedidos.Adapter.Adapter1ViewHolder, EstudosXamarim", Adapter1ViewHolder.class, __md_methods);
	}


	public Adapter1ViewHolder (android.view.View p0)
	{
		super (p0);
		if (getClass () == Adapter1ViewHolder.class)
			mono.android.TypeManager.Activate ("MeusPedidos.Adapter.Adapter1ViewHolder, EstudosXamarim", "Android.Views.View, Mono.Android", this, new java.lang.Object[] { p0 });
	}

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
