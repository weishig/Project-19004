﻿#pragma checksum "..\..\..\Views\ELLStageView.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "154024C7F5159E6629D2F350EC931786B7E4F185222FCDF9A93A531D999BD8A3"
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;
using Thorlabs.Elliptec.ELLO.Support;


namespace Thorlabs.Elliptec.ELLO.Views {
    
    
    /// <summary>
    /// ELLStageView
    /// </summary>
    public partial class ELLStageView : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 46 "..\..\..\Views\ELLStageView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox _homeOffset;
        
        #line default
        #line hidden
        
        
        #line 76 "..\..\..\Views\ELLStageView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox _jogStepSize;
        
        #line default
        #line hidden
        
        
        #line 169 "..\..\..\Views\ELLStageView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox _targetPosition;
        
        #line default
        #line hidden
        
        
        #line 191 "..\..\..\Views\ELLStageView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox _displacement;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Thorlabs.Elliptec.ELLO;component/views/ellstageview.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Views\ELLStageView.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal System.Delegate _CreateDelegate(System.Type delegateType, string handler) {
            return System.Delegate.CreateDelegate(delegateType, this, handler);
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this._homeOffset = ((System.Windows.Controls.TextBox)(target));
            
            #line 46 "..\..\..\Views\ELLStageView.xaml"
            this._homeOffset.PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(this.TextBox_PreviewTextInput);
            
            #line default
            #line hidden
            return;
            case 2:
            this._jogStepSize = ((System.Windows.Controls.TextBox)(target));
            
            #line 76 "..\..\..\Views\ELLStageView.xaml"
            this._jogStepSize.PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(this.TextBox_PreviewTextInput);
            
            #line default
            #line hidden
            return;
            case 3:
            this._targetPosition = ((System.Windows.Controls.TextBox)(target));
            
            #line 169 "..\..\..\Views\ELLStageView.xaml"
            this._targetPosition.PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(this.TextBox_PreviewTextInput);
            
            #line default
            #line hidden
            return;
            case 4:
            this._displacement = ((System.Windows.Controls.TextBox)(target));
            
            #line 191 "..\..\..\Views\ELLStageView.xaml"
            this._displacement.PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(this.TextBox_PreviewTextInput);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}
