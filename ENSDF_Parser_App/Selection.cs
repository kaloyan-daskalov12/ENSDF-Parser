using ENSDF_Parser.Extractor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

namespace ENSDF_Parser_App
{
    internal class Selection
    {
        public Selection(string name, object obj)
        {
            Name = name;
            Item = obj;
        }
        public string Name { get; private set; }

        public object Item { get; private set; }

        public Selection? Next { get; private set; }

        /// <summary>
        /// Gets an array of the properties that the current object contains
        /// </summary>
        /// <returns>A string array, containing the properties with they're types</returns>
        public string[] GetProperties()
        {
            if (Item is IList ilist)
            {
                return new string[] { $"The list contains {ilist.Count} elements!" };
            }
            PropertyInfo[] properties = Item.GetType().GetProperties();
            return properties.Select(p => $"[{p.GetValue(Item).GetType().Name}] {p.Name}").ToArray();
        }

        /// <summary>
        /// Creates a new <Selection> object that contains the specified property and adjust the <Next> property
        /// </summary>
        /// <param name="name">The name of the property; If it's a list, you can specify directly with index (ex. arr[index])</param>
        /// <returns>A reference to the new <Selection> item</returns>
        /// <exception cref="Exception"></exception>
        public Selection SelectSubitem(string name)
        {
            var target = Tools.ParseSquareBrackets(name);
            PropertyInfo? objProp = null;
            object? obj = null;
            Selection s;

            if (Item is IList ilist)
            {
                if (target.Args != null && target.Args.Count != 0)
                {
                    int index = int.Parse(target.Args[0]);
                    if (index >= ilist.Count || index < 0) throw new Exception($"The index {index} is out of range of {Name} entities!");
                    obj = ilist[index];
                    if (obj.GetType().IsPrimitive) throw new Exception($"Object of primitive type \"{obj.GetType().Name}\" can't be selected! Use \"set\" to assign value to it, or use \"show\" to see it's value!");
                    Item = obj;
                    Name = ((target.KeyWord == "") ? Name : target.KeyWord) + $"[{target.Args[0]}]";
                    return this;
                }
                else throw new Exception("The currently selected item is with a type of List; Select a specific item first!");
            }
            else objProp = Item.GetType().GetProperty(target.KeyWord);
            if (objProp == null) throw new Exception($"Object \"{target.KeyWord}\" doesn't exist in this context! Use \"dir\", to see the available fields!");
            obj = objProp.GetValue(Item);
            if (obj == null) throw new Exception($"Object \"{target.KeyWord}\" is not set!");
            if (obj.GetType().IsPrimitive) throw new Exception($"Object of primitive type \"{obj.GetType().Name}\" can't be selected! Use \"set\" to assign value to it, or use \"show\" to see it's value!");
            if (obj is IList list && target.Args != null && target.Args.Count != 0)
            {
                int index = int.Parse(target.Args[0]);
                if (index >= list.Count || index < 0) throw new Exception($"The index {index} is out of range of {Name} entities!");
                obj = list[index];
            }
            s = new Selection(name, obj);
            Next = s;
            return s;
        }

        /// <summary>
        /// Gets the value of the current object specified property
        /// </summary>
        /// <param name="property">The property, which value to get</param>
        /// <returns>The value of the property</returns>
        /// <exception cref="Exception"></exception>
        public object? GetValue(string property)
        {
            PropertyInfo? objProp = Item.GetType().GetProperty(property);
            if (objProp == null) throw new Exception($"Object \"{property}\" does not exist in the current context!");
            return objProp.GetValue(Item);
        }

        /// <summary>
        /// Sets a value to the specified property of the current object
        /// </summary>
        /// <param name="property">The property, which to set the value to</param>
        /// <param name="value">The value to be set</param>
        /// <exception cref="Exception"></exception>
        public void SetValue(string property, object value)
        {
            PropertyInfo? objProp = Item.GetType().GetProperty(property);
            if (objProp == null) throw new Exception($"Object \"{property}\" does not exist in the current context!");
            objProp.SetValue(Item, value);
        }

        /// <summary>
        /// Gets the last object in the chain
        /// </summary>
        /// <returns>The last object in the chain</returns>
        public Selection GetLast()
        {
            if (Next == null) return this;
            else return Next.GetLast();
        }

        public bool RemoveLast()
        {
            if (Next == null) return false;
            if (Next.Next == null) 
            {
                Next = null;
                return true; 
            }
            else return Next.RemoveLast();
        }

        public override string ToString()
        {
            if (Next == null) return $"{Name}";
            else return $"{Name}\\{Next}";
        }

        /// <summary>
        /// Creates a clone of the chain following the current <Selection> object, keeping the same reference to the Item
        /// </summary>
        /// <returns>The cloned object</returns>
        public Selection Clone()
        {
            Selection newSelection = (Selection)this.MemberwiseClone();
            if (Next != null) newSelection.Next = (Selection)Next.Clone();
            return newSelection;
        }
    }
}
