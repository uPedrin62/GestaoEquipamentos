﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="login.aspx.cs" Inherits="esoEquipamentos26.login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>ESO - Equipamentos</title>

    <link href="Content/bootstrap.min.css" rel="stylesheet" />

    <style>
        body {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            height: 100vh;
            margin: 0;
            display: flex;
            justify-content: center;
            align-items: center;
            position: relative;
            overflow: hidden;
        }

        .login-box {
            background-color: rgba(255, 255, 255, 0.95);
            padding: 40px;
            border-radius: 12px;
            box-shadow: 0 8px 32px rgba(0, 0, 0, 0.3);
            position: relative;
            z-index: 10;
            backdrop-filter: blur(10px);
        }

        .login-title {
            font-size: 1.5rem;
            font-weight: 600;
            text-align: center;
            margin-bottom: 30px;
        }

        .form-label {
            font-weight: 500;
        }

        .validation-message {
            color: #dc3545;
            font-size: 0.875rem;
        }
    </style>

    <script src="Scripts/particles.js"></script>

</head>
<body>
    <form id="form1" runat="server">
        <div>

            <div class="login-box">

                <table cellpadding="1" cellspacing="0" style="border-collapse: collapse;">
                    <tr>
                        <td>
                            <table cellpadding="0">
                                <tr style="height: 120px; vertical-align: middle;">
                                    <td align="center" colspan="2" class="fs-3">ESO - suporte informático</td>
                                </tr>
                                <tr style="height: 60px; vertical-align: middle;">
                                    <td align="right" style="padding-right: 10px;">
                                        <asp:Label ID="EmailLabel" runat="server" AssociatedControlID="Email">Email</asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="Email" runat="server"
                                            CssClass="form-control border-secondary" Width="300"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" ControlToValidate="Email" ErrorMessage="O email é obrigatório." ToolTip="O email é obrigatório." ValidationGroup="loginUtilizador"></asp:RequiredFieldValidator>
                                    </td>
                                </tr>
                                <tr style="height: 60px; vertical-align: middle;">
                                    <td align="right" style="padding-right: 10px;">
                                        <asp:Label ID="PasswordLabel" runat="server" AssociatedControlID="Password">Password:</asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="Password" runat="server" TextMode="Password"
                                            CssClass="form-control border-secondary"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="Password" ErrorMessage="Password é obrigatória." ToolTip="Password é obrigatória." ValidationGroup="loginUtilizador"></asp:RequiredFieldValidator>
                                    </td>
                                </tr>
                                <tr style="height: 80px; vertical-align: middle;">
                                    <td align="right" style="padding-right: 10px;"></td>
                                    <td align="left">
                                        <asp:Button ID="LoginButton" runat="server" Text="Login"
                                            ValidationGroup="loginUtilizador" CssClass="btn btn-outline-secondary"
                                            OnClick="LoginButton_Click" />
                                        <asp:Label ID="LoginError" runat="server" CssClass="text-danger" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>

            </div>

        </div>
    </form>
</body>
</html>