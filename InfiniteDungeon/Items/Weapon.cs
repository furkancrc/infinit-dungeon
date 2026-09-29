using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace InfiniteDungeon.Items
{
    abstract class Weapon : Item
    {
        private int damage;
        private String rarete;

        public Weapon(string name, string kind, int price, int damage, String rarete) : base(name, kind, price)
        {   
            this.damage = damage;
            this.rarete = rarete;
        }
    }
}
