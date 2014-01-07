VERSION 5.00
Begin VB.Form FileData 
   Appearance      =   0  'Flat
   BackColor       =   &H00404080&
   Caption         =   "File Data"
   ClientHeight    =   3750
   ClientLeft      =   4785
   ClientTop       =   3330
   ClientWidth     =   5895
   BeginProperty Font 
      Name            =   "MS Sans Serif"
      Size            =   8.25
      Charset         =   0
      Weight          =   700
      Underline       =   0   'False
      Italic          =   0   'False
      Strikethrough   =   0   'False
   EndProperty
   ForeColor       =   &H80000008&
   Icon            =   "FILEDATA.frx":0000
   LinkTopic       =   "Form1"
   MaxButton       =   0   'False
   MinButton       =   0   'False
   PaletteMode     =   1  'UseZOrder
   ScaleHeight     =   3750
   ScaleWidth      =   5895
   Begin VB.TextBox EditData 
      Appearance      =   0  'Flat
      BeginProperty Font 
         Name            =   "Courier"
         Size            =   9.75
         Charset         =   0
         Weight          =   700
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   2580
      Left            =   540
      MultiLine       =   -1  'True
      ScrollBars      =   3  'Both
      TabIndex        =   2
      Text            =   "FILEDATA.frx":0A8A
      Top             =   495
      Width           =   2940
   End
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
      Height          =   555
      Left            =   4410
      TabIndex        =   1
      Top             =   2070
      Width           =   960
   End
   Begin VB.CommandButton AcceptButton 
      Appearance      =   0  'Flat
      BackColor       =   &H80000005&
      Caption         =   "Accept"
      BeginProperty Font 
         Name            =   "MS Sans Serif"
         Size            =   9.75
         Charset         =   0
         Weight          =   700
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   555
      Left            =   4416
      TabIndex        =   0
      Top             =   960
      Width           =   960
   End
   Begin VB.Shape Shape2 
      BackColor       =   &H00C0C0C0&
      BackStyle       =   1  'Opaque
      Height          =   3030
      Left            =   4140
      Top             =   270
      Width           =   1545
   End
   Begin VB.Shape Shape1 
      BackColor       =   &H00C0C0C0&
      BackStyle       =   1  'Opaque
      Height          =   3030
      Left            =   270
      Top             =   270
      Width           =   3480
   End
End
Attribute VB_Name = "FileData"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Option Explicit

Private Sub AcceptButton_Click()
    Dim flag As Integer

    If (FileData.EditData.Text <> SaveEditData) Then
        flag = MsgBox("Update File?", 35, "RAYVB")
        If (flag = 2) Then  '// Cancel
            Exit Sub
        ElseIf (flag = 7) Then  '// No
            FileData.Hide
            Exit Sub
        Else   '// yes
            '// re-write the file
            ReplaceFile
        End If
    Else
        FileData.Hide
    End If
End Sub

Private Sub CancelButton_Click()
    '// Hide the Edit window
    FileData.Hide
End Sub

