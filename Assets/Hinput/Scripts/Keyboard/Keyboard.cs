using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ISKeyboard = UnityEngine.InputSystem.Keyboard;

namespace HinputClasses {
    /// <summary>
    /// Hinput class representing the keyboard. <BR/> <BR/>
    /// The properties of this class refer to the keys as positioned on a US keyboard, regardless of current keyboard
    /// layout. For instance, Hinput.keyboard.A always refers to the leftmost key of the second letter row. It maps
    /// to the A key of a standard US keyboard, but the Q key of a French keyboard.
    /// </summary>
    public class Keyboard : Device {
        // --------------------
        // PUBLIC PROPERTIES
        // --------------------
        
        /// <summary>
        /// The A key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key A;
        
        /// <summary>
        /// The B key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key B;
        
        /// <summary>
        /// The C key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key C;
        
        /// <summary>
        /// The D key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key D;
        
        /// <summary>
        /// The E key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key E;
        
        /// <summary>
        /// The F key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key F;
        
        /// <summary>
        /// The G key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key G;
        
        /// <summary>
        /// The H key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key H;
        
        /// <summary>
        /// The I key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key I;
        
        /// <summary>
        /// The J key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key J;
        
        /// <summary>
        /// The K key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key K;
        
        /// <summary>
        /// The L key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key L;
        
        /// <summary>
        /// The M key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key M;
        
        /// <summary>
        /// The N key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key N;
        
        /// <summary>
        /// The O key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key O;
        
        /// <summary>
        /// The P key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key P;
        
        /// <summary>
        /// The Q key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key Q;
        
        /// <summary>
        /// The R key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key R;
        
        /// <summary>
        /// The S key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key S;
        
        /// <summary>
        /// The T key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key T;
        
        /// <summary>
        /// The U key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key U;
        
        /// <summary>
        /// The V key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key V;
        
        /// <summary>
        /// The W key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key W;
        
        /// <summary>
        /// The X key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key X;
        
        /// <summary>
        /// The Y key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key Y;
        
        /// <summary>
        /// The Z key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key Z;
        
        
        /// <summary>
        /// The 0 key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key digit0;
        
        /// <summary>
        /// The 1 key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key digit1;
        
        /// <summary>
        /// The 2 key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key digit2;
        
        /// <summary>
        /// The 3 key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key digit3;
        
        /// <summary>
        /// The 4 key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key digit4;
        
        /// <summary>
        /// The 5 key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key digit5;
        
        /// <summary>
        /// The 6 key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key digit6;
        
        /// <summary>
        /// The 7 key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key digit7;
        
        /// <summary>
        /// The 8 key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key digit8;
        
        /// <summary>
        /// The 9 key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key digit9;
        
        
        /// <summary>
        /// The numpad 0 key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key numpad0;
        
        /// <summary>
        /// The numpad 1 key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key numpad1;
        
        /// <summary>
        /// The numpad 2 key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key numpad2;
        
        /// <summary>
        /// The numpad 3 key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key numpad3;
        
        /// <summary>
        /// The numpad 4 key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key numpad4;
        
        /// <summary>
        /// The numpad 5 key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key numpad5;
        
        /// <summary>
        /// The numpad 6 key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key numpad6;
        
        /// <summary>
        /// The numpad 7 key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key numpad7;
        
        /// <summary>
        /// The numpad 8 key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key numpad8;
        
        /// <summary>
        /// The numpad 9 key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key numpad9;
        
        /// <summary>
        /// The numlock key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key numLock;
        
        /// <summary>
        /// The enter key of the numpad (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key numpadEnter;
        
        /// <summary>
        /// The divide [/] key of the numpad (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key numpadDivide;
        
        /// <summary>
        /// The multiply [*] key of the numpad (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key numpadMultiply;
        
        /// <summary>
        /// The plus [+] key of the numpad (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key numpadPlus;
        
        /// <summary>
        /// The minus [-] key of the numpad (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key numpadMinus;
        
        /// <summary>
        /// The period [.] key of the numpad (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key numpadPeriod;
        
        /// <summary>
        /// The equals [=] key of the numpad (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key numpadEquals;

        /// <summary>
        /// The F1 key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key F1;
        
        /// <summary>
        /// The F2 key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key F2;
        
        /// <summary>
        /// The F3 key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key F3;
        
        /// <summary>
        /// The F4 key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key F4;
        
        /// <summary>
        /// The F5 key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key F5;
        
        /// <summary>
        /// The F6 key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key F6;
        
        /// <summary>
        /// The F7 key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key F7;
        
        /// <summary>
        /// The F8 key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key F8;
        
        /// <summary>
        /// The F9 key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key F9;
        
        /// <summary>
        /// The F10 key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key F10;
        
        /// <summary>
        /// The F11 key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key F11;
        
        /// <summary>
        /// The F12 key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key F12;
        
        /// <summary>
        /// The backquote [`] key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key backquote;
        
        /// <summary>
        /// The quote ['] key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key quote;
        
        /// <summary>
        /// The semicolon [;] key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key semicolon;
        
        /// <summary>
        /// The comma [,] key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key comma;
        
        /// <summary>
        /// The period [.] key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key period;
        
        /// <summary>
        /// The slash [/] key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key slash;
        
        /// <summary>
        /// The backslash [\] key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key backslash;
        
        /// <summary>
        /// The left bracket [(] key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key leftBracket;
        
        /// <summary>
        /// The right bracket [)] key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key rightBracket;
        
        /// <summary>
        /// The minus [-] key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key minus;
        
        /// <summary>
        /// The equals [-] key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key equals;
        
        /// <summary>
        /// The left shift key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key leftShift;
        
        /// <summary>
        /// The right shift key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key rightShift;
        
        /// <summary>
        /// The left alt key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key leftAlt;
        
        /// <summary>
        /// The right alt, or alt gr, key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key rightAlt;
        
        /// <summary>
        /// The left ctrl key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key leftCtrl;
        
        /// <summary>
        /// The right ctrl key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key rightCtrl;
        
        /// <summary>
        /// The left Windows key, the left Apple key or the left Command key, depending on the keyboard (as positioned
        /// on a standard US keyboard).
        /// </summary>
        public readonly Key leftCommand;
        
        /// <summary>
        /// The right Windows key, the right Apple key or the right Command key, depending on the keyboard (as positioned
        /// on a standard US keyboard).
        /// </summary>
        public readonly Key rightCommand;

        /// <summary>
        /// The space [ ] key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key space;
        
        /// <summary>
        /// The enter key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key enter;
        
        /// <summary>
        /// The tab [	] key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key tab;
        
        /// <summary>
        /// The caps lock key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key capsLock;
        
        /// <summary>
        /// The backspace key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key backSpace;
        
        /// <summary>
        /// The context menu key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key contextMenu;
        
        /// <summary>
        /// The escape key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key escape;
        
        /// <summary>
        /// The left arrow key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key leftArrow;
        
        /// <summary>
        /// The right arrow key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key rightArrow;
        
        /// <summary>
        /// The up arrow key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key upArrow;
        
        /// <summary>
        /// The down arrow key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key downArrow;
        
        /// <summary>
        /// The page down key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key pageDown;
        
        /// <summary>
        /// The page up key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key pageUp;
        
        /// <summary>
        /// The A key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key home;
        
        /// <summary>
        /// The end key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key end;
        
        /// <summary>
        /// The insert key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key insert;
        
        /// <summary>
        /// The delete key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key delete;
        
        /// <summary>
        /// The printscreen key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key printScreen;
        
        /// <summary>
        /// The scroll lock key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key scrollLock;
        
        /// <summary>
        /// The pause key (as positioned on a standard US keyboard).
        /// </summary>
        public readonly Key pause;

        /// <summary>
        /// The list containing keyboard keys, in the following order:<BR/> 
        /// - A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z,<BR/> 
        /// - digit0, digit1, digit2, digit3, digit4, digit5, digit6, digit7, digit8, digit9,<BR/> 
        /// - numpad0, numpad1, numpad2, numpad3, numpad4, numpad5, numpad6, numpad7, numpad8, numpad9, numLock,<BR/> 
        /// - numpadEnter, numpadDivide, numpadMultiply, numpadPlus, numpadMinus, numpadPeriod, numpadEquals,<BR/> 
        /// - F1, F2, F3, F4, F5, F6, F7, F8, F9, F10, F11, F12, <BR/> 
        /// - backquote, quote, semicolon, comma, period, slash, backslash, leftBracket, rightBracket, minus, equals,<BR/> 
        /// - leftShift, rightShift, leftAlt, rightAlt, leftCtrl, rightCtrl, leftCommand, rightCommand,<BR/> 
        /// - space, enter, tab, capsLock, backSpace, contextMenu, escape,<BR/> 
        /// - leftArrow, rightArrow, upArrow, downArrow,<BR/> 
        /// - pageDown, pageUp, home, end, insert, delete, printScreen, scrollLock, pause<BR/> 
        /// </summary>
        public readonly List<Key> keys;

        /// <summary>
        /// A shortcut returning the keyboard key that is currently being pressed.
        /// </summary>
        public Key currentInput => keys.FirstOrDefault(key => key.pressed);

        /// <summary>
        /// A virtual button returning every keyboard key at once.
        /// </summary>
        public readonly Key anyKey;
        
        
        // --------------------
        // CONSTRUCTOR
        // --------------------

        public Keyboard() {
            name = "Keyboard";
            
            A = new Key("a", this, ISKeyboard.current.aKey, KeyCode.A);
            B = new Key("b", this, ISKeyboard.current.bKey, KeyCode.B);
            C = new Key("c", this, ISKeyboard.current.cKey, KeyCode.C);
            D = new Key("d", this, ISKeyboard.current.dKey, KeyCode.D);
            E = new Key("e", this, ISKeyboard.current.eKey, KeyCode.E);
            F = new Key("f", this, ISKeyboard.current.fKey, KeyCode.F);
            G = new Key("g", this, ISKeyboard.current.gKey, KeyCode.G);
            H = new Key("h", this, ISKeyboard.current.hKey, KeyCode.H);
            I = new Key("i", this, ISKeyboard.current.iKey, KeyCode.I);
            J = new Key("j", this, ISKeyboard.current.jKey, KeyCode.J);
            K = new Key("k", this, ISKeyboard.current.kKey, KeyCode.K);
            L = new Key("l", this, ISKeyboard.current.lKey, KeyCode.L);
            M = new Key("m", this, ISKeyboard.current.mKey, KeyCode.M);
            N = new Key("n", this, ISKeyboard.current.nKey, KeyCode.N);
            O = new Key("o", this, ISKeyboard.current.oKey, KeyCode.O);
            P = new Key("p", this, ISKeyboard.current.pKey, KeyCode.P);
            Q = new Key("q", this, ISKeyboard.current.qKey, KeyCode.Q);
            R = new Key("r", this, ISKeyboard.current.rKey, KeyCode.R);
            S = new Key("s", this, ISKeyboard.current.sKey, KeyCode.S);
            T = new Key("t", this, ISKeyboard.current.tKey, KeyCode.T);
            U = new Key("u", this, ISKeyboard.current.uKey, KeyCode.U);
            V = new Key("v", this, ISKeyboard.current.vKey, KeyCode.V);
            W = new Key("w", this, ISKeyboard.current.wKey, KeyCode.W);
            X = new Key("x", this, ISKeyboard.current.xKey, KeyCode.X);
            Y = new Key("y", this, ISKeyboard.current.yKey, KeyCode.Y);
            Z = new Key("z", this, ISKeyboard.current.zKey, KeyCode.Z);
            
            digit0 = new Key("0", this, ISKeyboard.current.digit0Key, KeyCode.Alpha0);
            digit1 = new Key("1", this, ISKeyboard.current.digit1Key, KeyCode.Alpha1);
            digit2 = new Key("2", this, ISKeyboard.current.digit2Key, KeyCode.Alpha2);
            digit3 = new Key("3", this, ISKeyboard.current.digit3Key, KeyCode.Alpha3);
            digit4 = new Key("4", this, ISKeyboard.current.digit4Key, KeyCode.Alpha4);
            digit5 = new Key("5", this, ISKeyboard.current.digit5Key, KeyCode.Alpha5);
            digit6 = new Key("6", this, ISKeyboard.current.digit6Key, KeyCode.Alpha6);
            digit7 = new Key("7", this, ISKeyboard.current.digit7Key, KeyCode.Alpha7);
            digit8 = new Key("8", this, ISKeyboard.current.digit8Key, KeyCode.Alpha8);
            digit9 = new Key("9", this, ISKeyboard.current.digit9Key, KeyCode.Alpha9);
            
            numpad0 = new Key("0", this, ISKeyboard.current.numpad0Key, KeyCode.Keypad0);
            numpad1 = new Key("1", this, ISKeyboard.current.numpad1Key, KeyCode.Keypad1);
            numpad2 = new Key("2", this, ISKeyboard.current.numpad2Key, KeyCode.Keypad2);
            numpad3 = new Key("3", this, ISKeyboard.current.numpad3Key, KeyCode.Keypad3);
            numpad4 = new Key("4", this, ISKeyboard.current.numpad4Key, KeyCode.Keypad4);
            numpad5 = new Key("5", this, ISKeyboard.current.numpad5Key, KeyCode.Keypad5);
            numpad6 = new Key("6", this, ISKeyboard.current.numpad6Key, KeyCode.Keypad6);
            numpad7 = new Key("7", this, ISKeyboard.current.numpad7Key, KeyCode.Keypad7);
            numpad8 = new Key("8", this, ISKeyboard.current.numpad8Key, KeyCode.Keypad8);
            numpad9 = new Key("9", this, ISKeyboard.current.numpad9Key, KeyCode.Keypad9);
            numLock = new Key("", this, ISKeyboard.current.numLockKey, KeyCode.Numlock);
            
            numpadEnter = new Key("", this, ISKeyboard.current.numpadEnterKey, KeyCode.KeypadEnter);
            numpadDivide = new Key("/", this, ISKeyboard.current.numpadDivideKey, KeyCode.KeypadDivide);
            numpadMultiply = new Key("*", this, ISKeyboard.current.numpadMultiplyKey, KeyCode.KeypadMultiply);
            numpadPlus = new Key("+", this, ISKeyboard.current.numpadPlusKey, KeyCode.KeypadPlus);
            numpadMinus = new Key("-", this, ISKeyboard.current.numpadMinusKey, KeyCode.KeypadMinus);
            numpadPeriod = new Key(".", this, ISKeyboard.current.numpadPeriodKey, KeyCode.KeypadPeriod);
            numpadEquals = new Key("=", this, ISKeyboard.current.numpadEqualsKey, KeyCode.KeypadEquals);
            
            F1 = new Key("", this, ISKeyboard.current.f1Key, KeyCode.F1);
            F2 = new Key("", this, ISKeyboard.current.f2Key, KeyCode.F2);
            F3 = new Key("", this, ISKeyboard.current.f3Key, KeyCode.F3);
            F4 = new Key("", this, ISKeyboard.current.f4Key, KeyCode.F4);
            F5 = new Key("", this, ISKeyboard.current.f5Key, KeyCode.F5);
            F6 = new Key("", this, ISKeyboard.current.f6Key, KeyCode.F6);
            F7 = new Key("", this, ISKeyboard.current.f7Key, KeyCode.F7);
            F8 = new Key("", this, ISKeyboard.current.f8Key, KeyCode.F8);
            F9 = new Key("", this, ISKeyboard.current.f9Key, KeyCode.F9);
            F10 = new Key("", this, ISKeyboard.current.f10Key, KeyCode.F10);
            F11 = new Key("", this, ISKeyboard.current.f11Key, KeyCode.F11);
            F12 = new Key("", this, ISKeyboard.current.f12Key, KeyCode.F12);
            
            backquote = new Key("`", this, ISKeyboard.current.backquoteKey, KeyCode.BackQuote);
            quote = new Key("'", this, ISKeyboard.current.quoteKey, KeyCode.Quote);
            semicolon = new Key(";", this, ISKeyboard.current.semicolonKey, KeyCode.Semicolon);
            comma = new Key(",", this, ISKeyboard.current.commaKey, KeyCode.Comma);
            period = new Key(".", this, ISKeyboard.current.periodKey, KeyCode.Period);
            slash = new Key("/", this, ISKeyboard.current.slashKey, KeyCode.Slash);
            backslash = new Key("\\", this, ISKeyboard.current.backslashKey, KeyCode.Backslash);
            leftBracket = new Key("(", this, ISKeyboard.current.leftBracketKey, KeyCode.LeftBracket);
            rightBracket = new Key(")", this, ISKeyboard.current.rightBracketKey, KeyCode.RightBracket);
            minus = new Key("-", this, ISKeyboard.current.minusKey, KeyCode.Minus);
            equals = new Key("=", this, ISKeyboard.current.equalsKey, KeyCode.Equals);
            
            leftShift = new Key("", this, ISKeyboard.current.leftShiftKey, KeyCode.LeftShift);
            rightShift = new Key("", this, ISKeyboard.current.rightShiftKey, KeyCode.RightShift);
            leftAlt = new Key("", this, ISKeyboard.current.leftAltKey, KeyCode.LeftAlt);
            rightAlt = new Key("", this, ISKeyboard.current.rightAltKey, KeyCode.RightAlt);
            leftCtrl = new Key("", this, ISKeyboard.current.leftCtrlKey, KeyCode.LeftControl);
            rightCtrl = new Key("", this, ISKeyboard.current.rightCtrlKey, KeyCode.RightControl);
            leftCommand = new Key("", this, ISKeyboard.current.leftMetaKey, KeyCode.None);
            rightCommand = new Key("", this, ISKeyboard.current.rightMetaKey, KeyCode.None);
            
            space = new Key(" ", this, ISKeyboard.current.spaceKey, KeyCode.Space);
            enter = new Key("", this, ISKeyboard.current.enterKey, KeyCode.Return);
            tab = new Key("	", this, ISKeyboard.current.tabKey, KeyCode.Tab);
            capsLock = new Key("", this, ISKeyboard.current.capsLockKey, KeyCode.CapsLock);
            backSpace = new Key("", this, ISKeyboard.current.backspaceKey, KeyCode.Backspace);
            contextMenu = new Key("", this, ISKeyboard.current.contextMenuKey, KeyCode.None);
            escape = new Key("", this, ISKeyboard.current.escapeKey, KeyCode.Escape);
            
            leftArrow = new Key("", this, ISKeyboard.current.leftArrowKey, KeyCode.LeftArrow);
            rightArrow = new Key("", this, ISKeyboard.current.rightArrowKey, KeyCode.RightArrow);
            upArrow = new Key("", this, ISKeyboard.current.upArrowKey, KeyCode.UpArrow);
            downArrow = new Key("", this, ISKeyboard.current.downArrowKey, KeyCode.DownArrow);
            
            pageDown = new Key("", this, ISKeyboard.current.pageDownKey, KeyCode.PageDown);
            pageUp = new Key("", this, ISKeyboard.current.pageUpKey, KeyCode.PageUp);
            home = new Key("", this, ISKeyboard.current.homeKey, KeyCode.Home);
            end = new Key("", this, ISKeyboard.current.endKey, KeyCode.End);
            insert = new Key("", this, ISKeyboard.current.insertKey, KeyCode.Insert);
            delete = new Key("", this, ISKeyboard.current.deleteKey, KeyCode.Delete);
            printScreen = new Key("", this, ISKeyboard.current.printScreenKey, KeyCode.Print);
            scrollLock = new Key("", this, ISKeyboard.current.scrollLockKey, KeyCode.ScrollLock);
            pause = new Key("", this, ISKeyboard.current.pauseKey, KeyCode.Pause);
            
            keys = new List<Key> {
                A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z,
                digit0, digit1, digit2, digit3, digit4, digit5, digit6, digit7, digit8, digit9,
                numpad0, numpad1, numpad2, numpad3, numpad4, numpad5, numpad6, numpad7, numpad8, numpad9, numLock,
                numpadEnter, numpadDivide, numpadMultiply, numpadPlus, numpadMinus, numpadPeriod, numpadEquals,
                F1, F2, F3, F4, F5, F6, F7, F8, F9, F10, F11, F12, 
                backquote, quote, semicolon, comma, period, slash, backslash, leftBracket, rightBracket, minus, equals,
                leftShift, rightShift, leftAlt, rightAlt, leftCtrl, rightCtrl, leftCommand, rightCommand,
                space, enter, tab, capsLock, backSpace, contextMenu, escape,
                leftArrow, rightArrow, upArrow, downArrow,
                pageDown, pageUp, home, end, insert, delete, printScreen, scrollLock, pause
            };
            
            anyKey = new HinputClasses.Internal.AnyKey(this);
        }
        
        
        // --------------------
        // UPDATE
        // --------------------

        /// <summary>
        /// Hinput internal method.
        /// </summary>
        public void Update() {
            keys.ForEach(key => key.Update());
            anyKey.Update();
        }
    }
}
