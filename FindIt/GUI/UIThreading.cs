﻿using System;
using UnityEngine;
using ICities;
using ColossalFramework.UI;
using FindIt.GUI;

namespace FindIt
{
    public class UIThreading : ThreadingExtensionBase
    {
        // Flags.
        private bool _processed = false;

        // Keys
        private static KeyCode searchKey = (KeyCode)(Settings.searchKey.keyCode);
        private static KeyCode allKey = (KeyCode)(Settings.allKey.keyCode);
        private static KeyCode networkKey = (KeyCode)(Settings.networkKey.keyCode);
        private static KeyCode ploppableKey = (KeyCode)(Settings.ploppableKey.keyCode);
        private static KeyCode growableKey = (KeyCode)(Settings.growableKey.keyCode);
        private static KeyCode ricoKey = (KeyCode)(Settings.ricoKey.keyCode);
        private static KeyCode grwbRicoKey = (KeyCode)(Settings.grwbRicoKey.keyCode);
        private static KeyCode propKey = (KeyCode)(Settings.propKey.keyCode);
        private static KeyCode decalKey = (KeyCode)(Settings.decalKey.keyCode);
        private static KeyCode treeKey = (KeyCode)(Settings.treeKey.keyCode);
        private static KeyCode randomSelectionKey = (KeyCode)(Settings.randomSelectionKey.keyCode);

        /// <summary>
        /// Look for keypress to open GUI.
        /// </summary>
        /// <param name="realTimeDelta"></param>
        /// <param name="simulationTimeDelta"></param>
        public override void OnUpdate(float realTimeDelta, float simulationTimeDelta)
        {
            // Local references.
            UIButton mainButton = FindIt.instance?.mainButton;
            UISearchBox searchBox = FindIt.instance?.searchBox;
            if (searchBox == null || mainButton == null) return;

            // Null checks for safety.
            // Check modifier keys according to settings.
            bool altPressed = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt) || Input.GetKey(KeyCode.AltGr);
            bool ctrlPressed = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
            bool shiftPressed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

            // Has hotkey been pressed?
            if (searchKey != KeyCode.None && Input.GetKey(searchKey) && CheckHotkey(Settings.searchKey, altPressed, ctrlPressed, shiftPressed))
                OpenFindIt(-1);
            else if (allKey != KeyCode.None && Input.GetKey(allKey) && CheckHotkey(Settings.allKey, altPressed, ctrlPressed, shiftPressed))
                OpenFindIt(0);
            else if (networkKey != KeyCode.None && Input.GetKey(networkKey) && CheckHotkey(Settings.networkKey, altPressed, ctrlPressed, shiftPressed))
                OpenFindIt(1);
            else if (ploppableKey != KeyCode.None && Input.GetKey(ploppableKey) && CheckHotkey(Settings.ploppableKey, altPressed, ctrlPressed, shiftPressed))
                OpenFindIt(2);
            else if (growableKey != KeyCode.None && Input.GetKey(growableKey) && CheckHotkey(Settings.growableKey, altPressed, ctrlPressed, shiftPressed))
                OpenFindIt(3);
            else if (ricoKey != KeyCode.None && Input.GetKey(ricoKey) && CheckHotkey(Settings.ricoKey, altPressed, ctrlPressed, shiftPressed))
                OpenFindIt(4);
            else if (grwbRicoKey != KeyCode.None && Input.GetKey(grwbRicoKey) && CheckHotkey(Settings.grwbRicoKey, altPressed, ctrlPressed, shiftPressed))
                OpenFindIt(5);
            else if (propKey != KeyCode.None && Input.GetKey(propKey) && CheckHotkey(Settings.propKey, altPressed, ctrlPressed, shiftPressed))
                OpenFindIt(6);
            else if (decalKey != KeyCode.None && Input.GetKey(decalKey) && CheckHotkey(Settings.decalKey, altPressed, ctrlPressed, shiftPressed))
                OpenFindIt(7);
            else if (treeKey != KeyCode.None && Input.GetKey(treeKey) && CheckHotkey(Settings.treeKey, altPressed, ctrlPressed, shiftPressed))
                OpenFindIt(8);
            else if (randomSelectionKey != KeyCode.None && Input.GetKey(randomSelectionKey) && CheckHotkey(Settings.randomSelectionKey, altPressed, ctrlPressed, shiftPressed))
            {
                OpenFindIt(-2);
            }
            else
            {
                // Relevant keys aren't pressed anymore; this keystroke is over, so reset and continue.
                _processed = false;
            }

            // Check for escape press.
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (searchBox.hasFocus)
                {
                    // If the search box is focussed, unfocus.
                    searchBox.input.Unfocus();
                }
            }
        }

        public static bool CheckHotkey(KeyBinding keyBinding, bool altPressed, bool ctrlPressed, bool shiftPressed)
        {
            if ((altPressed != keyBinding.alt) || (ctrlPressed != keyBinding.control) || (shiftPressed != keyBinding.shift)) return false;
            return true;
        }

        public void OpenFindIt(int index)
        {
            // Cancel if key input is already queued for processing.
            if (_processed) return;

            _processed = true;
            try
            {
                // secondary keyboard shortcuts
                // if users choose to disable secondary hotkeys when Find it is invisible, don't do anything
                if (index != -1 && Settings.disableSecondaryKeyboardShortcuts && !FindIt.instance.searchBox.isVisible)
                {
                    return;
                }

                if (index > -1)
                {
                    if (index > 5 && !FindIt.isRicoEnabled)
                    {
                        index -= 2;
                    }
                    FindIt.instance.searchBox.typeFilter.selectedIndex = index;
                }

                // If the searchbox isn't visible, simulate a click on the main button.
                if (!FindIt.instance.searchBox.isVisible)
                {
                    FindIt.instance.mainButton.SimulateClick();
                }

                if (index == -2)
                {
                    UISearchBox.instance.PickRandom();
                }
                else
                {
                    // From Brot:
                    // Simulate a search
                    // Select search box text only if FindIt was opened via a category-specific hotkey or the "all"
                    // hotkey. This is intended to make overall behaviour more intuitive now that we're storing search
                    // queries separately for each asset category. This way, when you open FindIt directly to a specific
                    // category, you'll be all set up for starting a new search, but when you open it using the general
                    // hotkey, you'll start out ready for placement of whatever asset was last selected, without having
                    // to press Return first to get rid of the text selection.
                    if (index > -1)
                    {
                        FindIt.instance.searchBox.input.Focus();
                        FindIt.instance.searchBox.input.SelectAll();
                    }
                    else
                    {
                        FindIt.instance.searchBox.input.Focus(); // To-do. without the focus() the camera will move when F is pressed. need to avoid this.
                        FindIt.instance.searchBox.input.SelectAll(); // To-do
                        //FindIt.instance.searchBox.Search(); To-do
                    }
                }
            }
            catch (Exception e)
            {
                Debugging.LogException(e);
            }
        }

        /// <summary>
        /// Move It integration uses ctrl. Check if there is a hotkey collision
        /// Used in FindIt.OnButtonClicked
        /// returns true if collision happens
        /// </summary>
        public static bool CheckMoveItHotKeyCollision()
        {
            bool altPressed = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt) || Input.GetKey(KeyCode.AltGr);
            bool ctrlPressed = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
            bool shiftPressed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

            // if a hotkey is pressed
            if (searchKey != KeyCode.None && Input.GetKey(searchKey) && CheckHotkey(Settings.searchKey, altPressed, ctrlPressed, shiftPressed))
                return true;
            else if (allKey != KeyCode.None && Input.GetKey(allKey) && CheckHotkey(Settings.allKey, altPressed, ctrlPressed, shiftPressed))
                return true;
            else if (networkKey != KeyCode.None && Input.GetKey(networkKey) && CheckHotkey(Settings.networkKey, altPressed, ctrlPressed, shiftPressed))
                return true;
            else if (ploppableKey != KeyCode.None && Input.GetKey(ploppableKey) && CheckHotkey(Settings.ploppableKey, altPressed, ctrlPressed, shiftPressed))
                return true;
            else if (growableKey != KeyCode.None && Input.GetKey(growableKey) && CheckHotkey(Settings.growableKey, altPressed, ctrlPressed, shiftPressed))
                return true;
            else if (ricoKey != KeyCode.None && Input.GetKey(ricoKey) && CheckHotkey(Settings.ricoKey, altPressed, ctrlPressed, shiftPressed))
                return true;
            else if (grwbRicoKey != KeyCode.None && Input.GetKey(grwbRicoKey) && CheckHotkey(Settings.grwbRicoKey, altPressed, ctrlPressed, shiftPressed))
                return true;
            else if (propKey != KeyCode.None && Input.GetKey(propKey) && CheckHotkey(Settings.propKey, altPressed, ctrlPressed, shiftPressed))
                return true;
            else if (decalKey != KeyCode.None && Input.GetKey(decalKey) && CheckHotkey(Settings.decalKey, altPressed, ctrlPressed, shiftPressed))
                return true;
            else if (treeKey != KeyCode.None && Input.GetKey(treeKey) && CheckHotkey(Settings.treeKey, altPressed, ctrlPressed, shiftPressed))
                return true;
            else if (randomSelectionKey != KeyCode.None && Input.GetKey(randomSelectionKey) && CheckHotkey(Settings.randomSelectionKey, altPressed, ctrlPressed, shiftPressed))
                return true;

            return false;
        }

    }
}