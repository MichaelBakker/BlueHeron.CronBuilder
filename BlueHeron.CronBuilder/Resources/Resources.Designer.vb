﻿'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated by a tool.
'     Runtime Version:4.0.30319.42000
'
'     Changes to this file may cause incorrect behavior and will be lost if
'     the code is regenerated.
' </auto-generated>
'------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System

Namespace Localization
    
    'This class was auto-generated by the StronglyTypedResourceBuilder
    'class via a tool like ResGen or Visual Studio.
    'To add or remove a member, edit your .ResX file then rerun ResGen
    'with the /str option, or rebuild your VS project.
    '''<summary>
    '''  A strongly-typed resource class, for looking up localized strings, etc.
    '''</summary>
    <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0"),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Runtime.CompilerServices.CompilerGeneratedAttribute()>  _
    Public Class Resources
        
        Private Shared resourceMan As Global.System.Resources.ResourceManager
        
        Private Shared resourceCulture As Global.System.Globalization.CultureInfo
        
        <Global.System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")>  _
        Friend Sub New()
            MyBase.New
        End Sub
        
        '''<summary>
        '''  Returns the cached ResourceManager instance used by this class.
        '''</summary>
        <Global.System.ComponentModel.EditorBrowsableAttribute(Global.System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Public Shared ReadOnly Property ResourceManager() As Global.System.Resources.ResourceManager
            Get
                If Object.ReferenceEquals(resourceMan, Nothing) Then
                    Dim temp As Global.System.Resources.ResourceManager = New Global.System.Resources.ResourceManager("BlueHeron.Cron.Resources", GetType(Resources).Assembly)
                    resourceMan = temp
                End If
                Return resourceMan
            End Get
        End Property
        
        '''<summary>
        '''  Overrides the current thread's CurrentUICulture property for all
        '''  resource lookups using this strongly typed resource class.
        '''</summary>
        <Global.System.ComponentModel.EditorBrowsableAttribute(Global.System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Public Shared Property Culture() As Global.System.Globalization.CultureInfo
            Get
                Return resourceCulture
            End Get
            Set
                resourceCulture = value
            End Set
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to and.
        '''</summary>
        Public Shared ReadOnly Property _and() As String
            Get
                Return ResourceManager.GetString("and", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to and then.
        '''</summary>
        Public Shared ReadOnly Property andThen() As String
            Get
                Return ResourceManager.GetString("andThen", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to at.
        '''</summary>
        Public Shared ReadOnly Property atHour() As String
            Get
                Return ResourceManager.GetString("atHour", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to day.
        '''</summary>
        Public Shared ReadOnly Property day() As String
            Get
                Return ResourceManager.GetString("day", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to days.
        '''</summary>
        Public Shared ReadOnly Property days() As String
            Get
                Return ResourceManager.GetString("days", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to One or more errors occurred when validating parameter values..
        '''</summary>
        Public Shared ReadOnly Property errAggregateMessage() As String
            Get
                Return ResourceManager.GetString("errAggregateMessage", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Invalid number of arguments..
        '''</summary>
        Public Shared ReadOnly Property errArgumentCount() As String
            Get
                Return ResourceManager.GetString("errArgumentCount", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to &apos;{0}&apos; is invalid..
        '''</summary>
        Public Shared ReadOnly Property errParameter() As String
            Get
                Return ResourceManager.GetString("errParameter", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Invalid number of parameters..
        '''</summary>
        Public Shared ReadOnly Property errParameterCount() As String
            Get
                Return ResourceManager.GetString("errParameterCount", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Invalid ParameterValueType value: &apos;{0}&apos;..
        '''</summary>
        Public Shared ReadOnly Property errParameterValueType() As String
            Get
                Return ResourceManager.GetString("errParameterValueType", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Value types of &apos;{0}&apos; do not match..
        '''</summary>
        Public Shared ReadOnly Property errParameterValueTypes() As String
            Get
                Return ResourceManager.GetString("errParameterValueTypes", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to &apos;{0}&apos; is invalid. {1}.
        '''</summary>
        Public Shared ReadOnly Property errParameterWithMessage() As String
            Get
                Return ResourceManager.GetString("errParameterWithMessage", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to every.
        '''</summary>
        Public Shared ReadOnly Property every() As String
            Get
                Return ResourceManager.GetString("every", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to friday.
        '''</summary>
        Public Shared ReadOnly Property friday() As String
            Get
                Return ResourceManager.GetString("friday", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to hour.
        '''</summary>
        Public Shared ReadOnly Property hour() As String
            Get
                Return ResourceManager.GetString("hour", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to hours.
        '''</summary>
        Public Shared ReadOnly Property hours() As String
            Get
                Return ResourceManager.GetString("hours", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to in.
        '''</summary>
        Public Shared ReadOnly Property inMonth() As String
            Get
                Return ResourceManager.GetString("inMonth", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to minute.
        '''</summary>
        Public Shared ReadOnly Property minute() As String
            Get
                Return ResourceManager.GetString("minute", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to minutes.
        '''</summary>
        Public Shared ReadOnly Property minutes() As String
            Get
                Return ResourceManager.GetString("minutes", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to monday.
        '''</summary>
        Public Shared ReadOnly Property monday() As String
            Get
                Return ResourceManager.GetString("monday", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to month.
        '''</summary>
        Public Shared ReadOnly Property month() As String
            Get
                Return ResourceManager.GetString("month", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to months.
        '''</summary>
        Public Shared ReadOnly Property months() As String
            Get
                Return ResourceManager.GetString("months", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to on.
        '''</summary>
        Public Shared ReadOnly Property onDay() As String
            Get
                Return ResourceManager.GetString("onDay", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to saturday.
        '''</summary>
        Public Shared ReadOnly Property saturday() As String
            Get
                Return ResourceManager.GetString("saturday", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to starting.
        '''</summary>
        Public Shared ReadOnly Property starting() As String
            Get
                Return ResourceManager.GetString("starting", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to starting with.
        '''</summary>
        Public Shared ReadOnly Property startingWith() As String
            Get
                Return ResourceManager.GetString("startingWith", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to sunday.
        '''</summary>
        Public Shared ReadOnly Property sunday() As String
            Get
                Return ResourceManager.GetString("sunday", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to through.
        '''</summary>
        Public Shared ReadOnly Property through() As String
            Get
                Return ResourceManager.GetString("through", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to thursday.
        '''</summary>
        Public Shared ReadOnly Property thursday() As String
            Get
                Return ResourceManager.GetString("thursday", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to tuesday.
        '''</summary>
        Public Shared ReadOnly Property tuesday() As String
            Get
                Return ResourceManager.GetString("tuesday", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to wednesday.
        '''</summary>
        Public Shared ReadOnly Property wednesday() As String
            Get
                Return ResourceManager.GetString("wednesday", resourceCulture)
            End Get
        End Property
    End Class
End Namespace
