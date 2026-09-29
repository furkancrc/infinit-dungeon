using System;
using System.Collections.Generic;
using System.Text;
using InfiniteDungeon.Items;

namespace InfiniteDungeon.Entity
{
    internal class Enemy
    {

        private int damage { get; set; }

        private List<Item> inventory { get; set; }

        public void Attack(Player target)
        {
            target.getHealth() = (target.getHealth() - this.damage);
        }

        public List<Item> DropLoot()
        {

        }
    }
}
