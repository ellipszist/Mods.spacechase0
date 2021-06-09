using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;

namespace GenericModConfigMenu.UI
{
    public abstract class Container : Element
    {
        private IList<Element> children = new List<Element>();

        public Element RenderLast { get; set; }

        public Element[] Children { get { return this.children.ToArray(); } }

        public void AddChild(Element element)
        {
            element.Parent?.RemoveChild(element);
            this.children.Add(element);
            element.Parent = this;
        }

        public void RemoveChild(Element element)
        {
            if (element.Parent != this)
                throw new ArgumentException("Element must be a child of this container.");
            this.children.Remove(element);
            element.Parent = null;
        }

        public override void Draw(SpriteBatch b)
        {
            foreach (var child in this.children)
            {
                if (child == this.RenderLast)
                    continue;
                child.Draw(b);
            }
            if (this.RenderLast != null)
                this.RenderLast.Draw(b);
        }
    }
}
