VERSION 5.00
Begin VB.Form RegNum 
   Appearance      =   0  'Flat
   BackColor       =   &H00404080&
   Caption         =   "Registration Numbers"
   ClientHeight    =   3765
   ClientLeft      =   1455
   ClientTop       =   1590
   ClientWidth     =   4995
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
   Icon            =   "REGNUM.frx":0000
   LinkTopic       =   "Form2"
   MaxButton       =   0   'False
   PaletteMode     =   1  'UseZOrder
   ScaleHeight     =   3765
   ScaleWidth      =   4995
   Begin VB.Timer TMTimer 
      Interval        =   50
      Left            =   3780
      Top             =   1500
   End
   Begin VB.CommandButton ExitButton 
      Appearance      =   0  'Flat
      BackColor       =   &H80000005&
      Caption         =   "Exit"
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
      Left            =   3540
      TabIndex        =   2
      Top             =   1980
      Width           =   915
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
      Left            =   3540
      TabIndex        =   1
      Top             =   840
      Width           =   915
   End
   Begin VB.ListBox RegList 
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
      Height          =   2760
      Left            =   300
      TabIndex        =   0
      Top             =   300
      Width           =   2640
   End
   Begin VB.Label DriverLab 
      Appearance      =   0  'Flat
      BackColor       =   &H00404080&
      ForeColor       =   &H80000008&
      Height          =   375
      Left            =   180
      TabIndex        =   3
      Top             =   3240
      Width           =   4695
      WordWrap        =   -1  'True
   End
   Begin VB.Shape Shape1 
      BackColor       =   &H00C0C0C0&
      BackStyle       =   1  'Opaque
      Height          =   2892
      Left            =   3300
      Top             =   276
      Width           =   1320
   End
End
Attribute VB_Name = "RegNum"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Option Explicit


Private Sub ExitButton_Click()
    '// End the program when click Exit Button
    End
End Sub

Private Sub Form_Unload(Cancel As Integer)
    End
End Sub


Private Sub RegList_DblClick()
    Dim RS As String

    '// attempt to get the file list of the choosen device
    RS = RegList.List(RegList.ListIndex)
    GetFileList RS
End Sub

Private Sub SelectButton_Click()
    Dim RS As String

    '// check to see that one is first selected
    If (RegList.ListIndex = -1) Then
        Exit Sub
    End If

    '// attempt to get the file list of the choosen device
    RS = RegList.List(RegList.ListIndex)
    GetFileList RS
End Sub

'// This Timer routine keeps an updated list of Registration Numbers from
'// Touch Memory on the One-Wire.
'//
Private Sub TMTimer_Timer()
    '// declarations
    Dim search, flag, I, J As Integer
    ReDim ROM(8) As Integer
    Dim RS As String

    '// start a session
    SHandle = TMExtendedStartSession(PortNum, PortType, vbNullString)

    '// if SHandle valid then search for a device
    If (SHandle > 0) Then
        '// check to see if setup needs to be done
        If (Not SetupDone) Then
            flag = TMSetup(SHandle)
            SetupDone = True
        End If
        
        '// look for the 'next' device on the 1-Wire
        search = TMNext(SHandle, state_buffer(0))

        If (search = 1) Then
            '// device found, so get its ROM code
            ROM(0) = 0
            flag = TMRom(SHandle, state_buffer(0), ROM(0))

            '// create a string out of ROM number
            RS = ""
            For I = 7 To 0 Step -1
                If (ROM(I) <= &HF) Then RS = RS + "0"
                RS = RS + Hex$(ROM(I))
            Next I
            
            '// check for rom in list
            flag = False
            For I = 0 To RegList.ListCount - 1
                If (RS = RegList.List(I)) Then
                    '// found in list so update debounce
                    Debounce(I) = MaxDbnc
                    flag = True
                    Exit For
                End If
            Next I
            
            '// if not found then add
            If (Not flag) Then
                RegList.AddItem RS
                Beep
                RegList.ListIndex = RegList.ListCount - 1
                Debounce(I) = MaxDbnc
            End If
        '// Next found no parts so at end of list
        Else
            '// age devices
            If (TMValidSession(SHandle) = 1) Then
                For I = 0 To RegList.ListCount - 1
                    If (Debounce(I) > 0) Then
                        Debounce(I) = Debounce(I) - 1
                    End If
                Next I
            End If
            '// check for parts that have left
            I = 0
            Do While (I < RegList.ListCount)
                If (Debounce(I) <= 0) Then
                    RegList.RemoveItem I
                    Beep
                    '// shift the Debounce values up
                    For J = I To RegList.ListCount - 1
                        Debounce(I) = Debounce(I + 1)
                    Next J
                End If
                I = I + 1
            Loop
        End If
    End If
    '// end the current session
    flag = TMEndSession(SHandle)
End Sub

