namespace SmartEstate.Email;

public static class EmailTemplates
{
    public static string GetPasswordRecoveryTemplate(string recoveryLink)
    {
        return $@"
        <!DOCTYPE html>
        <html>
        <head>
            <style>
                body {{ font-family: Arial, sans-serif; line-height: 1.6; }}
                .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                .button {{ 
                    display: inline-block; 
                    padding: 10px 20px; 
                    background-color: #4CAF50; 
                    color: white; 
                    text-decoration: none; 
                    border-radius: 5px;
                }}
            </style>
        </head>
        <body>
            <div class='container'>
                <h2>Восстановление пароля</h2>
                <p>Вы получили это письмо, потому что был запрошен сброс пароля для вашего аккаунта.</p>
                <p>Пожалуйста, нажмите на кнопку ниже, чтобы установить новый пароль:</p>
                <p><a href='{recoveryLink}' class='button'>Восстановить пароль</a></p>
                <p>Если вы не запрашивали сброс пароля, просто проигнорируйте это письмо.</p>
                <p>Ссылка действительна в течение 24 часов.</p>
                <p>С уважением,<br>Команда SmartEstate</p>
            </div>
        </body>
        </html>";
    }
    
    public static string GetWelcomeEmailTemplate()
    {
        return $@"
        <!DOCTYPE html>
        <html>
        <head>
            <style>
                body {{ font-family: Arial, sans-serif; line-height: 1.6; }}
                .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                .button {{ 
                    display: inline-block; 
                    padding: 10px 20px; 
                    background-color: #4CAF50; 
                    color: white; 
                    text-decoration: none; 
                    border-radius: 5px;
                }}
            </style>
        </head>
        <body>
            <div class='container'>
                <h2>Приветственное письмо</h2>
                <p>Благодарим вас за регистрацию в SmartEstate!</p>
                <p>Если вы не регистрировались в нашем сервисе, пожалуйста, проигнорируйте это письмо.</p>
                <p>С уважением,<br>Команда SmartEstate</p>
            </div>
        </body>
        </html>";
    }
    
    public static string GetChangeEmailTemplate(string userName)
    {
        return $@"
        <!DOCTYPE html>
        <html>
        <head>
            <style>
                body {{ font-family: Arial, sans-serif; line-height: 1.6; }}
                .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                .button {{ 
                    display: inline-block; 
                    padding: 10px 20px; 
                    background-color: #4CAF50; 
                    color: white; 
                    text-decoration: none; 
                    border-radius: 5px;
                }}
            </style>
        </head>
        <body>
            <div class='container'>
                <h2>Смена электронной почты</h2>
                <p>Почта от аккаунта {userName} в SmartEstate изменена на данную!</p>
                <p>Если вы не меняли почту в нашем сервисе, пожалуйста, проигнорируйте это письмо.</p>
                <p>С уважением,<br>Команда SmartEstate</p>
            </div>
        </body>
        </html>";
    }
}