﻿#pragma checksum "C:\RealWorld\Windows10\Chapter13\Lesson13_BigMountainX\BigMountainX\TicketingPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "E8F098CD2E948D9B5FF7BF1975B6A389"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BigMountainX
{
    partial class TicketingPage : 
        global::Windows.UI.Xaml.Controls.Page, 
        global::Windows.UI.Xaml.Markup.IComponentConnector,
        global::Windows.UI.Xaml.Markup.IComponentConnector2
    {
        /// <summary>
        /// Connect()
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 14.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                {
                    this.btn_ticket = (global::Windows.UI.Xaml.Controls.Button)(target);
                    #line 15 "..\..\..\TicketingPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.Button)this.btn_ticket).Click += this.OnStandardTicket;
                    #line default
                }
                break;
            case 2:
                {
                    this.btn_vipticket = (global::Windows.UI.Xaml.Controls.Button)(target);
                    #line 20 "..\..\..\TicketingPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.Button)this.btn_vipticket).Click += this.OnVIPTicket;
                    #line default
                }
                break;
            case 3:
                {
                    this.btn_openmic = (global::Windows.UI.Xaml.Controls.Button)(target);
                    #line 25 "..\..\..\TicketingPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.Button)this.btn_openmic).Click += this.OnOpenMicTicket;
                    #line default
                }
                break;
            default:
                break;
            }
            this._contentLoaded = true;
        }

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 14.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::Windows.UI.Xaml.Markup.IComponentConnector GetBindingConnector(int connectionId, object target)
        {
            global::Windows.UI.Xaml.Markup.IComponentConnector returnValue = null;
            return returnValue;
        }
    }
}

