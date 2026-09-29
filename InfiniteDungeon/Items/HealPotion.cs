using System;
using System.Collections.Generic;
using System.Text;

namespace InfiniteDungeon.Items
{
    abstract class HealPotion : Item
    {
        private int heal;
        private String rarete;

        public HealPotion(string name, string kind, int price, int heal, String rarete) : base(name, kind, price)
        {
            this.heal = heal;
            this.rarete = rarete;
        }
    }
}
