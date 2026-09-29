using InfiniteDungeon.Entity;

namespace InfiniteDungeon.Items
{
    public abstract class Item
    {
        public string name;
        private string kind;
        public int price;

        protected Item(string name, string kind, int price)
        {
            this.name = name;
            this.kind = kind;
            this.price = price;
        }
    }
}