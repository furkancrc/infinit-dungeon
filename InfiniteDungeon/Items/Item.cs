using InfiniteDungeon.Entity;

namespace InfiniteDungeon.Items
{
    public abstract class Item
    {
        public string Name;
        private string Kind;
        public int Price;


        protected Item(string name, string kind, int price)
        {
            this.Name = name;
            this.Kind = kind;
            this.Price = price;
        }
    }
}