VERSION 5.00
Begin VB.Form DirList 
   Appearance      =   0  'Flat
   BackColor       =   &H00404080&
   Caption         =   "File Names"
   ClientHeight    =   3570
   ClientLeft      =   3300
   ClientTop       =   5400
   ClientWidth     =   4620
   ForeColor       =   &H80000008&
   Icon            =   "DIRLIST.frx":0000
   LinkTopic       =   "Form1"
   MaxButton       =   0   'False
   MinButton       =   0   'False
   PaletteMode     =   1  'UseZOrder
   ScaleHeight     =   3570
   ScaleWidth      =   4620
   Begin VB.CommandButton CancelButton 
      Appearance      =   0  'Flat
      BackColor       =   &H80000005&
      Caption         =   "Cancel"
      BeginProperty Font 
         Name            =   "MS Sans Serif"
         Size            =   9.75
         Charset         =   0
         Weight          =   700
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   600
      Left            =   2970
      TabIndex        =   2
      Top             =   1890
      Width           =   960
   End
   Begin VB.CommandButton SelectButton 
      Appearance      =   0  'Flat
      BackColor       =   &H80000005&
      Caption         =   "Select"
      BeginProperty Font 
         Name            =   "MS Sans Serif"
         Size            =   9.75
         Charset         =   0
         Weight          =   700
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   600
      Left            =   2976
      TabIndex        =   1
      Top             =   840
      Width           =   960
   End
   Begin VB.ListBox FileList 
      Appearance      =   0  'Flat
      BackColor       =   &H00C0C0C0&
      BeginProperty Font 
         Name            =   "Courier"
         Size            =   9.75
         Charset         =   0
         Weight          =   700
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   2955
      Left            =   360
      TabIndex        =   0
      Top             =   180
      Width           =   1815
   End
   Begin VB.Shape Shape1 
      BackColor       =   &H00C0C0C0&
      FillColor       =   &H00C0C0C0&
      FillStyle       =   0  'Solid
      Height          =   2895
      Left            =   2640
      Top             =   180
      Width           =   1545
   End
End
Attribute VB_Name = "DirList"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Option Explicit

Private Sub CancelButton_Click()
    '// Hide the Directory List Window
    Hide
End Sub

Private Sub FileList_DblClick()
    Dim RS As String

    '// check to see that one is first selected
    If (FileList.ListIndex = -1) Then
        Exit Sub
    End If

    '// get the file name
    RS = FileList.List(FileList.ListIndex)
    
    '// check to see if it is a sub directory
    If (Right$(RS, 5) = "<DIR>") Then
        ChangeDirectory RS
    Else
        EditFileData RS
    End If
End Sub

Private Sub SelectButton_Click()
    Dim RS As String

    '// check to see that one is first selected
    If (FileList.ListIndex = -1) Then
        Exit Sub
    End If

    '// get the file name
    RS = FileList.List(FileList.ListIndex)

    '// check to see if it is a sub directory
    If (Right$(RS, 5) = "<DIR>") Then
        ChangeDirectory RS
    Else
        EditFileData RS
    End If
End Sub

