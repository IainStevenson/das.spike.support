﻿using System.Collections.Generic;

namespace Spike.Support.Shared.Models
{
    public class Menu
    {
        /// <summary>
        ///     Provides raw presentation and navigational data for a Menu
        /// </summary>
        public List<NavItem> NavItems { get; set; } = new List<NavItem>();

        /// <summary>
        ///     Declares which of the
        ///     <param>
        ///         <name>ActiveMenuItemKeys</name>
        ///     </param>
        ///     is the currently selected one
        /// </summary>
        public string ActiveMenuRootKey { get; set; }

        /// <summary>
        ///     A sequential list of keys found in the current journey through the menu
        ///     <param>
        ///         <name>NavItems</name>
        ///     </param>
        /// </summary>
        public List<string> ActiveMenuItemKeys { get; set; } = new List<string>();

        /// <summary>
        ///     The current level of the menu, influences the choice or menu orientation x % 2 = Vertical else horizontal
        /// </summary>
        public int Level { get; set; }

        public MenuOrientations MenuOrientation { get; set; }

        public static Menu ConfigureMenu(List<NavItem> menuNavItems,
            string selectedRoot,
            List<string> selectedItems,
            MenuOrientations orientation)
        {
            return new Menu
            {
                ActiveMenuItemKeys = selectedItems,
                ActiveMenuRootKey = selectedRoot,
                NavItems = menuNavItems,
                MenuOrientation = orientation
            };
        }
    }
}