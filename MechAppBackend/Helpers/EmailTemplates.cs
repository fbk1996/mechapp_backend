using MechAppBackend.AppSettings;

namespace MechAppBackend.Helpers
{
    public class EmailTemplates
    {
        public static string AddEmployeeTitle = "Oficjalne powitanie! Twoje Konto jest Gotowe!";

        public static string AddEmployeeTemplate = @"<!DOCTYPE html>
<html lang='pl' xmlns='http://www.w3.org/1999/xhtml' xmlns:v='urn:schemas-microsoft-com:vml'
	xmlns:o='urn:schemas-microsoft-com:office:office'>

<head>
	<meta charset='utf-8'> <!-- utf-8 works for most cases -->
	<meta name='viewport' content='width=device-width'> <!-- Forcing initial-scale shouldn't be necessary -->
	<meta http-equiv='X-UA-Compatible' content='IE=edge'> <!-- Use the latest (edge) version of IE rendering engine -->
	<meta name='x-apple-disable-message-reformatting'> <!-- Disable auto-scale in iOS 10 Mail entirely -->
	<title>#title</title> <!-- The title tag shows in email notifications, like Android 4.4. -->
	<link href='https://fonts.googleapis.com/css?family=Poppins:200,300,400,500,600,700' rel='stylesheet'>
	<!-- CSS Reset : BEGIN -->
	<style>
		/* What it does: Remove spaces around the email design added by some email clients. */
		/* Beware: It can remove the padding / margin and add a background color to the compose a reply window. */
		html,
		body {
			margin: 0 auto !important;
			padding: 0 !important;
			height: 100% !important;
			width: 100% !important;
			background: #f1f1f1;
		}

		/* What it does: Stops email clients resizing small text. */
		* {
			-ms-text-size-adjust: 100%;
			-webkit-text-size-adjust: 100%;
		}

		/* What it does: Centers email on Android 4.4 */
		div[style*='margin: 16px 0'] {
			margin: 0 !important;
		}

		/* What it does: Stops Outlook from adding extra spacing to tables. */
		table,
		td {
			mso-table-lspace: 0pt !important;
			mso-table-rspace: 0pt !important;
		}

		/* What it does: Fixes webkit padding issue. */
		table {
			border-spacing: 0 !important;
			border-collapse: collapse !important;
			table-layout: fixed !important;
			margin: 0 auto !important;
		}

		/* What it does: Uses a better rendering method when resizing images in IE. */
		img {
			-ms-interpolation-mode: bicubic;
		}

		/* What it does: Prevents Windows 10 Mail from underlining links despite inline CSS. Styles for underlined links should be inline. */
		a {
			text-decoration: none;
		}

		/* What it does: A work-around for email clients meddling in triggered links. */
		*[x-apple-data-detectors],
		/* iOS */
		.unstyle-auto-detected-links *,
		.aBn {
			border-bottom: 0 !important;
			cursor: default !important;
			color: inherit !important;
			text-decoration: none !important;
			font-size: inherit !important;
			font-family: inherit !important;
			font-weight: inherit !important;
			line-height: inherit !important;
		}

		/* What it does: Prevents Gmail from displaying a download button on large, non-linked images. */
		.a6S {
			display: none !important;
			opacity: 0.01 !important;
		}

		/* What it does: Prevents Gmail from changing the text color in conversation threads. */
		.im {
			color: inherit !important;
		}

		/* If the above doesn't work, add a .g-img class to any image in question. */
		img.g-img+div {
			display: none !important;
		}

		/* What it does: Removes right gutter in Gmail iOS app: https://github.com/TedGoas/Cerberus/issues/89  */
		/* Create one of these media queries for each additional viewport size you'd like to fix */
		/* iPhone 4, 4S, 5, 5S, 5C, and 5SE */
		@media only screen and (min-device-width: 320px) and (max-device-width: 374px) {
			u~div .email-container {
				min-width: 320px !important;
			}
		}

		/* iPhone 6, 6S, 7, 8, and X */
		@media only screen and (min-device-width: 375px) and (max-device-width: 413px) {
			u~div .email-container {
				min-width: 375px !important;
			}
		}

		/* iPhone 6+, 7+, and 8+ */
		@media only screen and (min-device-width: 414px) {
			u~div .email-container {
				min-width: 414px !important;
			}
		}
	</style> <!-- CSS Reset : END -->
	<style>
		.primary {
			background: #17bebb;
		}

		.bg_white {
			background: #ffffff;
		}

		.bg_light {
			background: #f7fafa;
		}

		.bg_black {
			background: #000000;
		}

		.bg_dark {
			background: rgba(0, 0, 0, .8);
		}

		.email-section {
			padding: 2.5em;
		}

		/*BUTTON*/
		.btn {
			padding: 10px 15px;
			display: inline-block;
		}

		.btn.btn-primary {
			border-radius: 5px;
			background: #17bebb;
			color: #ffffff;
		}

		.btn.btn-white {
			border-radius: 5px;
			background: #ffffff;
			color: #000000;
		}

		.btn.btn-white-outline {
			border-radius: 5px;
			background: transparent;
			border: 1px solid #fff;
			color: #fff;
		}

		.btn.btn-black-outline {
			border-radius: 0px;
			background: transparent;
			border: 2px solid #000;
			color: #000;
			font-weight: 700;
		}

		.btn-custom {
			color: rgba(0, 0, 0, .3);
			text-decoration: underline;
		}

		h1,
		h2,
		h3,
		h4,
		h5,
		h6 {
			font-family: 'Poppins', sans-serif;
			color: #000000;
			margin-top: 0;
			font-weight: 400;
		}

		body {
			font-family: 'Poppins', sans-serif;
			font-weight: 400;
			font-size: 15px;
			line-height: 1.8;
			color: rgba(0, 0, 0, .4);
		}

		a {
			color: #17bebb;
		}

		table {}

		/*LOGO*/
		.logo h1 {
			margin: 0;
		}

		.logo h1 a {
			color: #17bebb;
			font-size: 24px;
			font-weight: 700;
			font-family: 'Poppins', sans-serif;
		}

		/*HERO*/
		.hero {
			position: relative;
			z-index: 0;
		}

		.hero .text {
			color: rgba(0, 0, 0, .3);
		}

		.hero .text h2 {
			color: #000;
			font-size: 34px;
			margin-bottom: 0;
			font-weight: 200;
			line-height: 1.4;
		}

		.hero .text h3 {
			font-size: 24px;
			font-weight: 300;
		}

		.hero .text h2 span {
			font-weight: 600;
			color: #000;
		}

		.text-author {
			bordeR: 1px solid rgba(0, 0, 0, .05);
			max-width: 50%;
			margin: 0 auto;
			padding: 2em;
		}

		.text-author img {
			border-radius: 50%;
			padding-bottom: 20px;
		}

		.text-author h3 {
			margin-bottom: 0;
		}

		ul.social {
			padding: 0;
		}

		ul.social li {
			display: inline-block;
			margin-right: 10px;
		}

		/*FOOTER*/
		.footer {
			border-top: 1px solid rgba(0, 0, 0, .05);
			color: rgba(0, 0, 0, .5);
		}

		.footer .heading {
			color: #000;
			font-size: 20px;
		}

		.footer ul {
			margin: 0;
			padding: 0;
		}

		.footer ul li {
			list-style: none;
			margin-bottom: 10px;
		}

		.footer ul li a {
			color: rgba(0, 0, 0, 1);
		}

		@media screen and (max-width: 500px) {}
	</style> <!-- CSS Reset : END -->
	<!-- Progressive Enhancements : BEGIN -->
	<style>
		.primary {
			background: #17bebb;
		}

		.bg_white {
			background: #ffffff;
		}

		.bg_light {
			background: #f7fafa;
		}

		.bg_black {
			background: #000000;
		}

		.bg_dark {
			background: rgba(0, 0, 0, .8);
		}

		.email-section {
			padding: 2.5em;
		}

		/*BUTTON*/
		.btn {
			padding: 10px 15px;
			display: inline-block;
		}

		.btn.btn-primary {
			border-radius: 5px;
			background: #17bebb;
			color: #ffffff;
		}

		.btn.btn-white {
			border-radius: 5px;
			background: #ffffff;
			color: #000000;
		}

		.btn.btn-white-outline {
			border-radius: 5px;
			background: transparent;
			border: 1px solid #fff;
			color: #fff;
		}

		.btn.btn-black-outline {
			border-radius: 0px;
			background: transparent;
			border: 2px solid #000;
			color: #000;
			font-weight: 700;
		}

		.btn-custom {
			color: rgba(0, 0, 0, .3);
			text-decoration: underline;
		}

		h1,
		h2,
		h3,
		h4,
		h5,
		h6 {
			font-family: 'Poppins', sans-serif;
			color: #000000;
			margin-top: 0;
			font-weight: 400;
		}

		body {
			font-family: 'Poppins', sans-serif;
			font-weight: 400;
			font-size: 15px;
			line-height: 1.8;
			color: rgba(0, 0, 0, .4);
		}

		a {
			color: #17bebb;
		}

		table {}

		/*LOGO*/
		.logo h1 {
			margin: 0;
		}

		.logo h1 a {
			color: #17bebb;
			font-size: 24px;
			font-weight: 700;
			font-family: 'Poppins', sans-serif;
		}

		/*HERO*/
		.hero {
			position: relative;
			z-index: 0;
		}

		.hero .text {
			color: rgba(0, 0, 0, .3);
		}

		.hero .text h2 {
			color: #000;
			font-size: 34px;
			margin-bottom: 0;
			font-weight: 200;
			line-height: 1.4;
		}

		.hero .text h3 {
			font-size: 24px;
			font-weight: 300;
		}

		.hero .text h2 span {
			font-weight: 600;
			color: #000;
		}

		.text-author {
			bordeR: 1px solid rgba(0, 0, 0, .05);
			max-width: 50%;
			margin: 0 auto;
			padding: 2em;
		}

		.text-author img {
			border-radius: 50%;
			padding-bottom: 20px;
		}

		.text-author h3 {
			margin-bottom: 0;
		}

		ul.social {
			padding: 0;
		}

		ul.social li {
			display: inline-block;
			margin-right: 10px;
		}

		/*FOOTER*/
		.footer {
			border-top: 1px solid rgba(0, 0, 0, .05);
			color: rgba(0, 0, 0, .5);
		}

		.footer .heading {
			color: #000;
			font-size: 20px;
		}

		.footer ul {
			margin: 0;
			padding: 0;
		}

		.footer ul li {
			list-style: none;
			margin-bottom: 10px;
		}

		.footer ul li a {
			color: rgba(0, 0, 0, 1);
		}

	</style>
</head>

<body width='100%' style='margin: 0; padding: 0 !important; mso-line-height-rule: exactly; background-color: #f1f1f1;'>
	<center style='width: 100%;'>
		<div
			style='display: none; font-size: 1px;max-height: 0px; max-width: 0px; opacity: 0; overflow: hidden; mso-hide: all; font-family: sans-serif;'>
			&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;
		</div>
		<div style='max-width: 600px; margin: 0 auto;' class='email-container'>
			<!-- BEGIN BODY -->
			<table align='center' role='presentation' cellspacing='0' cellpadding='0' border='0' width='100%'
				style='margin: auto;'>
				<tr>
					<td valign='top' class='bg_white' style='padding: 1em 2.5em 0 2.5em;'>
						<table role='presentation' border='0' cellpadding='0' cellspacing='0' width='100%'>
							<tr>
								<td class='logo' style='text-align: center;'>
									<h1><a href='#'><img src='" + appdata.emailLogoUrl + @"'
												style='width: 250px; height: auto;'></a></h1>
								</td>
							</tr>
						</table>
					</td>
				</tr><!-- end tr -->
				<tr>
					<td valign='middle' class='hero bg_white' style='padding: 2em 0 4em 0;'>
						<table role='presentation' border='0' cellpadding='0' cellspacing='0' width='100%'>
							<tr>
								<td style='padding: 0 2.5em; text-align: center; padding-bottom: 3em;'>
									<div class='text'>
										<h2>Witaj #name #lastname<br /><br />Twoje konto zostało utworzone!</h2><br>
                                        <h5>Chcieliśmy Cię serdecznie przywitać w naszym zespole!<br />Jesteśmy bardzo zadowoleni, że jesteś z nami.
                                            Jeśli masz pytania, nie wahaj się z nami skontaktować</h5>
									</div>
								</td>
							</tr>
							<tr>
								<td style='text-align: center;'>
									<div class='text-author'>
										<h3 class='name'>Twoje dane logowania</h3> <br />
										<h4 class='position'>Login: #login</h4>
										<h4 class='position'>Hasło: #password</h4>
										<p><a href='" + appdata.loginUrl + @"' class='btn btn-primary'>Przejdź do logowania</a></p>
									</div>
								</td>
							</tr>
						</table>
					</td>
				</tr><!-- end tr -->
				<!-- 1 Column Text + Button : END -->
			</table>
			<table align='center' role='presentation' cellspacing='0' cellpadding='0' border='0' width='100%'
				style='margin: auto;'>
				<tr>
					<td valign='middle' class='bg_light footer email-section'>
						<table>
							<tr>
								<td valign='top' width='50%' style='padding-top: 20px;'>
									<table role='presentation' cellspacing='0' cellpadding='0' border='0' width='100%'>
										<tr>
											<td style='text-align: left; padding-left: 5px; padding-right: 5px;'>
												<h3 class='heading'>Kontakt:</h3>
												<ul>
													<li><span class='text'>" + appdata.companyName + @".<br />" + appdata.companyAddress + @"<br />" + appdata.companyPostcode + @" " + appdata.companyCity + @"</span></li>
													<li><span class='text'>+48 " + appdata.companyPhone + @"</span></li>
                                                    <li><span class='text'>" + appdata.companyEmail + @"</span></li>
												</ul>
											</td>
										</tr>
									</table>
								</td>
							</tr>
						</table>
					</td>
				</tr><!-- end: tr -->
				<tr> </tr>
			</table>
		</div>
	</center>
</body>

</html>";

        public static string GreetExistingAccountEmployee = @"<!DOCTYPE html>
<html lang='pl' xmlns='http://www.w3.org/1999/xhtml' xmlns:v='urn:schemas-microsoft-com:vml'
	xmlns:o='urn:schemas-microsoft-com:office:office'>

<head>
	<meta charset='utf-8'> <!-- utf-8 works for most cases -->
	<meta name='viewport' content='width=device-width'> <!-- Forcing initial-scale shouldn't be necessary -->
	<meta http-equiv='X-UA-Compatible' content='IE=edge'> <!-- Use the latest (edge) version of IE rendering engine -->
	<meta name='x-apple-disable-message-reformatting'> <!-- Disable auto-scale in iOS 10 Mail entirely -->
	<title>#title</title> <!-- The title tag shows in email notifications, like Android 4.4. -->
	<link href='https://fonts.googleapis.com/css?family=Poppins:200,300,400,500,600,700' rel='stylesheet'>
	<!-- CSS Reset : BEGIN -->
	<style>
		/* What it does: Remove spaces around the email design added by some email clients. */
		/* Beware: It can remove the padding / margin and add a background color to the compose a reply window. */
		html,
		body {
			margin: 0 auto !important;
			padding: 0 !important;
			height: 100% !important;
			width: 100% !important;
			background: #f1f1f1;
		}

		/* What it does: Stops email clients resizing small text. */
		* {
			-ms-text-size-adjust: 100%;
			-webkit-text-size-adjust: 100%;
		}

		/* What it does: Centers email on Android 4.4 */
		div[style*='margin: 16px 0'] {
			margin: 0 !important;
		}

		/* What it does: Stops Outlook from adding extra spacing to tables. */
		table,
		td {
			mso-table-lspace: 0pt !important;
			mso-table-rspace: 0pt !important;
		}

		/* What it does: Fixes webkit padding issue. */
		table {
			border-spacing: 0 !important;
			border-collapse: collapse !important;
			table-layout: fixed !important;
			margin: 0 auto !important;
		}

		/* What it does: Uses a better rendering method when resizing images in IE. */
		img {
			-ms-interpolation-mode: bicubic;
		}

		/* What it does: Prevents Windows 10 Mail from underlining links despite inline CSS. Styles for underlined links should be inline. */
		a {
			text-decoration: none;
		}

		/* What it does: A work-around for email clients meddling in triggered links. */
		*[x-apple-data-detectors],
		/* iOS */
		.unstyle-auto-detected-links *,
		.aBn {
			border-bottom: 0 !important;
			cursor: default !important;
			color: inherit !important;
			text-decoration: none !important;
			font-size: inherit !important;
			font-family: inherit !important;
			font-weight: inherit !important;
			line-height: inherit !important;
		}

		/* What it does: Prevents Gmail from displaying a download button on large, non-linked images. */
		.a6S {
			display: none !important;
			opacity: 0.01 !important;
		}

		/* What it does: Prevents Gmail from changing the text color in conversation threads. */
		.im {
			color: inherit !important;
		}

		/* If the above doesn't work, add a .g-img class to any image in question. */
		img.g-img+div {
			display: none !important;
		}

		/* What it does: Removes right gutter in Gmail iOS app: https://github.com/TedGoas/Cerberus/issues/89  */
		/* Create one of these media queries for each additional viewport size you'd like to fix */
		/* iPhone 4, 4S, 5, 5S, 5C, and 5SE */
		@media only screen and (min-device-width: 320px) and (max-device-width: 374px) {
			u~div .email-container {
				min-width: 320px !important;
			}
		}

		/* iPhone 6, 6S, 7, 8, and X */
		@media only screen and (min-device-width: 375px) and (max-device-width: 413px) {
			u~div .email-container {
				min-width: 375px !important;
			}
		}

		/* iPhone 6+, 7+, and 8+ */
		@media only screen and (min-device-width: 414px) {
			u~div .email-container {
				min-width: 414px !important;
			}
		}
	</style> <!-- CSS Reset : END -->
	<style>
		.primary {
			background: #17bebb;
		}

		.bg_white {
			background: #ffffff;
		}

		.bg_light {
			background: #f7fafa;
		}

		.bg_black {
			background: #000000;
		}

		.bg_dark {
			background: rgba(0, 0, 0, .8);
		}

		.email-section {
			padding: 2.5em;
		}

		/*BUTTON*/
		.btn {
			padding: 10px 15px;
			display: inline-block;
		}

		.btn.btn-primary {
			border-radius: 5px;
			background: #17bebb;
			color: #ffffff;
		}

		.btn.btn-white {
			border-radius: 5px;
			background: #ffffff;
			color: #000000;
		}

		.btn.btn-white-outline {
			border-radius: 5px;
			background: transparent;
			border: 1px solid #fff;
			color: #fff;
		}

		.btn.btn-black-outline {
			border-radius: 0px;
			background: transparent;
			border: 2px solid #000;
			color: #000;
			font-weight: 700;
		}

		.btn-custom {
			color: rgba(0, 0, 0, .3);
			text-decoration: underline;
		}

		h1,
		h2,
		h3,
		h4,
		h5,
		h6 {
			font-family: 'Poppins', sans-serif;
			color: #000000;
			margin-top: 0;
			font-weight: 400;
		}

		body {
			font-family: 'Poppins', sans-serif;
			font-weight: 400;
			font-size: 15px;
			line-height: 1.8;
			color: rgba(0, 0, 0, .4);
		}

		a {
			color: #17bebb;
		}

		table {}

		/*LOGO*/
		.logo h1 {
			margin: 0;
		}

		.logo h1 a {
			color: #17bebb;
			font-size: 24px;
			font-weight: 700;
			font-family: 'Poppins', sans-serif;
		}

		/*HERO*/
		.hero {
			position: relative;
			z-index: 0;
		}

		.hero .text {
			color: rgba(0, 0, 0, .3);
		}

		.hero .text h2 {
			color: #000;
			font-size: 34px;
			margin-bottom: 0;
			font-weight: 200;
			line-height: 1.4;
		}

		.hero .text h3 {
			font-size: 24px;
			font-weight: 300;
		}

		.hero .text h2 span {
			font-weight: 600;
			color: #000;
		}

		.text-author {
			bordeR: 1px solid rgba(0, 0, 0, .05);
			max-width: 50%;
			margin: 0 auto;
			padding: 2em;
		}

		.text-author img {
			border-radius: 50%;
			padding-bottom: 20px;
		}

		.text-author h3 {
			margin-bottom: 0;
		}

		ul.social {
			padding: 0;
		}

		ul.social li {
			display: inline-block;
			margin-right: 10px;
		}

		/*FOOTER*/
		.footer {
			border-top: 1px solid rgba(0, 0, 0, .05);
			color: rgba(0, 0, 0, .5);
		}

		.footer .heading {
			color: #000;
			font-size: 20px;
		}

		.footer ul {
			margin: 0;
			padding: 0;
		}

		.footer ul li {
			list-style: none;
			margin-bottom: 10px;
		}

		.footer ul li a {
			color: rgba(0, 0, 0, 1);
		}

		@media screen and (max-width: 500px) {}
	</style> <!-- CSS Reset : END -->
	<!-- Progressive Enhancements : BEGIN -->
	<style>
		.primary {
			background: #17bebb;
		}

		.bg_white {
			background: #ffffff;
		}

		.bg_light {
			background: #f7fafa;
		}

		.bg_black {
			background: #000000;
		}

		.bg_dark {
			background: rgba(0, 0, 0, .8);
		}

		.email-section {
			padding: 2.5em;
		}

		/*BUTTON*/
		.btn {
			padding: 10px 15px;
			display: inline-block;
		}

		.btn.btn-primary {
			border-radius: 5px;
			background: #17bebb;
			color: #ffffff;
		}

		.btn.btn-white {
			border-radius: 5px;
			background: #ffffff;
			color: #000000;
		}

		.btn.btn-white-outline {
			border-radius: 5px;
			background: transparent;
			border: 1px solid #fff;
			color: #fff;
		}

		.btn.btn-black-outline {
			border-radius: 0px;
			background: transparent;
			border: 2px solid #000;
			color: #000;
			font-weight: 700;
		}

		.btn-custom {
			color: rgba(0, 0, 0, .3);
			text-decoration: underline;
		}

		h1,
		h2,
		h3,
		h4,
		h5,
		h6 {
			font-family: 'Poppins', sans-serif;
			color: #000000;
			margin-top: 0;
			font-weight: 400;
		}

		body {
			font-family: 'Poppins', sans-serif;
			font-weight: 400;
			font-size: 15px;
			line-height: 1.8;
			color: rgba(0, 0, 0, .4);
		}

		a {
			color: #17bebb;
		}

		table {}

		/*LOGO*/
		.logo h1 {
			margin: 0;
		}

		.logo h1 a {
			color: #17bebb;
			font-size: 24px;
			font-weight: 700;
			font-family: 'Poppins', sans-serif;
		}

		/*HERO*/
		.hero {
			position: relative;
			z-index: 0;
		}

		.hero .text {
			color: rgba(0, 0, 0, .3);
		}

		.hero .text h2 {
			color: #000;
			font-size: 34px;
			margin-bottom: 0;
			font-weight: 200;
			line-height: 1.4;
		}

		.hero .text h3 {
			font-size: 24px;
			font-weight: 300;
		}

		.hero .text h2 span {
			font-weight: 600;
			color: #000;
		}

		.text-author {
			bordeR: 1px solid rgba(0, 0, 0, .05);
			max-width: 50%;
			margin: 0 auto;
			padding: 2em;
		}

		.text-author img {
			border-radius: 50%;
			padding-bottom: 20px;
		}

		.text-author h3 {
			margin-bottom: 0;
		}

		ul.social {
			padding: 0;
		}

		ul.social li {
			display: inline-block;
			margin-right: 10px;
		}

		/*FOOTER*/
		.footer {
			border-top: 1px solid rgba(0, 0, 0, .05);
			color: rgba(0, 0, 0, .5);
		}

		.footer .heading {
			color: #000;
			font-size: 20px;
		}

		.footer ul {
			margin: 0;
			padding: 0;
		}

		.footer ul li {
			list-style: none;
			margin-bottom: 10px;
		}

		.footer ul li a {
			color: rgba(0, 0, 0, 1);
		}

	</style>
</head>

<body width='100%' style='margin: 0; padding: 0 !important; mso-line-height-rule: exactly; background-color: #f1f1f1;'>
	<center style='width: 100%;'>
		<div
			style='display: none; font-size: 1px;max-height: 0px; max-width: 0px; opacity: 0; overflow: hidden; mso-hide: all; font-family: sans-serif;'>
			&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;
		</div>
		<div style='max-width: 600px; margin: 0 auto;' class='email-container'>
			<!-- BEGIN BODY -->
			<table align='center' role='presentation' cellspacing='0' cellpadding='0' border='0' width='100%'
				style='margin: auto;'>
				<tr>
					<td valign='top' class='bg_white' style='padding: 1em 2.5em 0 2.5em;'>
						<table role='presentation' border='0' cellpadding='0' cellspacing='0' width='100%'>
							<tr>
								<td class='logo' style='text-align: center;'>
									<h1><a href='#'><img src='" + appdata.emailLogoUrl + @"'
												style='width: 250px; height: auto;'></a></h1>
								</td>
							</tr>
						</table>
					</td>
				</tr><!-- end tr -->
				<tr>
					<td valign='middle' class='hero bg_white' style='padding: 2em 0 4em 0;'>
						<table role='presentation' border='0' cellpadding='0' cellspacing='0' width='100%'>
							<tr>
								<td style='padding: 0 2.5em; text-align: center; padding-bottom: 3em;'>
									<div class='text'>
										<h2>Witaj #name&nbsp;#lastname<br /></h2><br>
                                        <h5>Chcieliśmy Cię serdecznie przywitać w naszym zespole!<br />Jesteśmy bardzo zadowoleni, że jesteś z nami.
                                            Jeśli masz pytania, nie wahaj się z nami skontaktować</h5>
									</div>
								</td>
							</tr>
							<tr>
								<td style='text-align: center;'>
									<div class='text-author'>
										<h3 class='name'>Już teraz możesz zacząć korzystać z naszej platformy. <br> Zaloguj się i odkryj wszystkie dostępne funkcje, które pomogą Ci w pracy:</h3> <br />
										<p><a href='" + appdata.loginUrl + @"' class='btn btn-primary'>Przejdź do logowania</a></p>
									</div>
								</td>
							</tr>
						</table>
					</td>
				</tr><!-- end tr -->
				<!-- 1 Column Text + Button : END -->
			</table>
			<table align='center' role='presentation' cellspacing='0' cellpadding='0' border='0' width='100%'
				style='margin: auto;'>
				<tr>
					<td valign='middle' class='bg_light footer email-section'>
						<table>
							<tr>
								<td valign='top' width='50%' style='padding-top: 20px;'>
									<table role='presentation' cellspacing='0' cellpadding='0' border='0' width='100%'>
										<tr>
											<td style='text-align: left; padding-left: 5px; padding-right: 5px;'>
												<h3 class='heading'>Kontakt:</h3>
												<ul>
													<li><span class='text'>" + appdata.companyName + @".<br />" + appdata.companyAddress + @"<br />" + appdata.companyPostcode + @" " + appdata.companyCity + @"</span></li>
													<li><span class='text'>+48 " + appdata.companyPhone + @"</span></li>
                                                    <li><span class='text'>" + appdata.companyEmail + @"</span></li>
												</ul>
											</td>
										</tr>
									</table>
								</td>
							</tr>
						</table>
					</td>
				</tr><!-- end: tr -->
				<tr> </tr>
			</table>
		</div>
	</center>
</body>

</html>";

        public static string validationCodeMailTitle = $"{appdata.companyName} - Kod autoryzacyjny";

        public static string validationCodeMailMessage = @"<!DOCTYPE html>
<html lang='pl' xmlns='http://www.w3.org/1999/xhtml' xmlns:v='urn:schemas-microsoft-com:vml'
	xmlns:o='urn:schemas-microsoft-com:office:office'>

<head>
	<meta charset='utf-8'> <!-- utf-8 works for most cases -->
	<meta name='viewport' content='width=device-width'> <!-- Forcing initial-scale shouldn't be necessary -->
	<meta http-equiv='X-UA-Compatible' content='IE=edge'> <!-- Use the latest (edge) version of IE rendering engine -->
	<meta name='x-apple-disable-message-reformatting'> <!-- Disable auto-scale in iOS 10 Mail entirely -->
	<title>@Model.Title</title> <!-- The title tag shows in email notifications, like Android 4.4. -->
	<link href='https://fonts.googleapis.com/css?family=Poppins:200,300,400,500,600,700' rel='stylesheet'>
	<!-- CSS Reset : BEGIN -->
	<style>
		/* What it does: Remove spaces around the email design added by some email clients. */
		/* Beware: It can remove the padding / margin and add a background color to the compose a reply window. */
		html,
		body {
			margin: 0 auto !important;
			padding: 0 !important;
			height: 100% !important;
			width: 100% !important;
			background: #f1f1f1;
		}

		/* What it does: Stops email clients resizing small text. */
		* {
			-ms-text-size-adjust: 100%;
			-webkit-text-size-adjust: 100%;
		}

		/* What it does: Centers email on Android 4.4 */
		div[style*='margin: 16px 0'] {
			margin: 0 !important;
		}

		/* What it does: Stops Outlook from adding extra spacing to tables. */
		table,
		td {
			mso-table-lspace: 0pt !important;
			mso-table-rspace: 0pt !important;
		}

		/* What it does: Fixes webkit padding issue. */
		table {
			border-spacing: 0 !important;
			border-collapse: collapse !important;
			table-layout: fixed !important;
			margin: 0 auto !important;
		}

		/* What it does: Uses a better rendering method when resizing images in IE. */
		img {
			-ms-interpolation-mode: bicubic;
		}

		/* What it does: Prevents Windows 10 Mail from underlining links despite inline CSS. Styles for underlined links should be inline. */
		a {
			text-decoration: none;
		}

		/* What it does: A work-around for email clients meddling in triggered links. */
		*[x-apple-data-detectors],
		/* iOS */
		.unstyle-auto-detected-links *,
		.aBn {
			border-bottom: 0 !important;
			cursor: default !important;
			color: inherit !important;
			text-decoration: none !important;
			font-size: inherit !important;
			font-family: inherit !important;
			font-weight: inherit !important;
			line-height: inherit !important;
		}

		/* What it does: Prevents Gmail from displaying a download button on large, non-linked images. */
		.a6S {
			display: none !important;
			opacity: 0.01 !important;
		}

		/* What it does: Prevents Gmail from changing the text color in conversation threads. */
		.im {
			color: inherit !important;
		}

		/* If the above doesn't work, add a .g-img class to any image in question. */
		img.g-img+div {
			display: none !important;
		}

		/* What it does: Removes right gutter in Gmail iOS app: https://github.com/TedGoas/Cerberus/issues/89  */
		/* Create one of these media queries for each additional viewport size you'd like to fix */
		/* iPhone 4, 4S, 5, 5S, 5C, and 5SE */
		@media only screen and (min-device-width: 320px) and (max-device-width: 374px) {
			u~div .email-container {
				min-width: 320px !important;
			}
		}

		/* iPhone 6, 6S, 7, 8, and X */
		@media only screen and (min-device-width: 375px) and (max-device-width: 413px) {
			u~div .email-container {
				min-width: 375px !important;
			}
		}

		/* iPhone 6+, 7+, and 8+ */
		@media only screen and (min-device-width: 414px) {
			u~div .email-container {
				min-width: 414px !important;
			}
		}
	</style> <!-- CSS Reset : END -->
	<style>
		.primary {
			background: #17bebb;
		}

		.bg_white {
			background: #ffffff;
		}

		.bg_light {
			background: #f7fafa;
		}

		.bg_black {
			background: #000000;
		}

		.bg_dark {
			background: rgba(0, 0, 0, .8);
		}

		.email-section {
			padding: 2.5em;
		}

		/*BUTTON*/
		.btn {
			padding: 10px 15px;
			display: inline-block;
		}

		.btn.btn-primary {
			border-radius: 5px;
			background: #17bebb;
			color: #ffffff;
		}

		.btn.btn-white {
			border-radius: 5px;
			background: #ffffff;
			color: #000000;
		}

		.btn.btn-white-outline {
			border-radius: 5px;
			background: transparent;
			border: 1px solid #fff;
			color: #fff;
		}

		.btn.btn-black-outline {
			border-radius: 0px;
			background: transparent;
			border: 2px solid #000;
			color: #000;
			font-weight: 700;
		}

		.btn-custom {
			color: rgba(0, 0, 0, .3);
			text-decoration: underline;
		}

		h1,
		h2,
		h3,
		h4,
		h5,
		h6 {
			font-family: 'Poppins', sans-serif;
			color: #000000;
			margin-top: 0;
			font-weight: 400;
		}

		body {
			font-family: 'Poppins', sans-serif;
			font-weight: 400;
			font-size: 15px;
			line-height: 1.8;
			color: rgba(0, 0, 0, .4);
		}

		a {
			color: #17bebb;
		}

		table {}

		/*LOGO*/
		.logo h1 {
			margin: 0;
		}

		.logo h1 a {
			color: #17bebb;
			font-size: 24px;
			font-weight: 700;
			font-family: 'Poppins', sans-serif;
		}

		/*HERO*/
		.hero {
			position: relative;
			z-index: 0;
		}

		.hero .text {
			color: rgba(0, 0, 0, .3);
		}

		.hero .text h2 {
			color: #000;
			font-size: 34px;
			margin-bottom: 0;
			font-weight: 200;
			line-height: 1.4;
		}

		.hero .text h3 {
			font-size: 24px;
			font-weight: 300;
		}

		.hero .text h2 span {
			font-weight: 600;
			color: #000;
		}

		.text-author {
			bordeR: 1px solid rgba(0, 0, 0, .05);
			max-width: 50%;
			margin: 0 auto;
			padding: 2em;
		}

		.text-author img {
			border-radius: 50%;
			padding-bottom: 20px;
		}

		.text-author h3 {
			margin-bottom: 0;
		}

		ul.social {
			padding: 0;
		}

		ul.social li {
			display: inline-block;
			margin-right: 10px;
		}

		/*FOOTER*/
		.footer {
			border-top: 1px solid rgba(0, 0, 0, .05);
			color: rgba(0, 0, 0, .5);
		}

		.footer .heading {
			color: #000;
			font-size: 20px;
		}

		.footer ul {
			margin: 0;
			padding: 0;
		}

		.footer ul li {
			list-style: none;
			margin-bottom: 10px;
		}

		.footer ul li a {
			color: rgba(0, 0, 0, 1);
		}

		@media screen and (max-width: 500px) {}
	</style> <!-- CSS Reset : END -->
	<!-- Progressive Enhancements : BEGIN -->
	<style>
		.primary {
			background: #17bebb;
		}

		.bg_white {
			background: #ffffff;
		}

		.bg_light {
			background: #f7fafa;
		}

		.bg_black {
			background: #000000;
		}

		.bg_dark {
			background: rgba(0, 0, 0, .8);
		}

		.email-section {
			padding: 2.5em;
		}

		/*BUTTON*/
		.btn {
			padding: 10px 15px;
			display: inline-block;
		}

		.btn.btn-primary {
			border-radius: 5px;
			background: #17bebb;
			color: #ffffff;
		}

		.btn.btn-white {
			border-radius: 5px;
			background: #ffffff;
			color: #000000;
		}

		.btn.btn-white-outline {
			border-radius: 5px;
			background: transparent;
			border: 1px solid #fff;
			color: #fff;
		}

		.btn.btn-black-outline {
			border-radius: 0px;
			background: transparent;
			border: 2px solid #000;
			color: #000;
			font-weight: 700;
		}

		.btn-custom {
			color: rgba(0, 0, 0, .3);
			text-decoration: underline;
		}

		h1,
		h2,
		h3,
		h4,
		h5,
		h6 {
			font-family: 'Poppins', sans-serif;
			color: #000000;
			margin-top: 0;
			font-weight: 400;
		}

		body {
			font-family: 'Poppins', sans-serif;
			font-weight: 400;
			font-size: 15px;
			line-height: 1.8;
			color: rgba(0, 0, 0, .4);
		}

		a {
			color: #17bebb;
		}

		table {}

		/*LOGO*/
		.logo h1 {
			margin: 0;
		}

		.logo h1 a {
			color: #17bebb;
			font-size: 24px;
			font-weight: 700;
			font-family: 'Poppins', sans-serif;
		}

		/*HERO*/
		.hero {
			position: relative;
			z-index: 0;
		}

		.hero .text {
			color: rgba(0, 0, 0, .3);
		}

		.hero .text h2 {
			color: #000;
			font-size: 34px;
			margin-bottom: 0;
			font-weight: 200;
			line-height: 1.4;
		}

		.hero .text h3 {
			font-size: 24px;
			font-weight: 300;
		}

		.hero .text h2 span {
			font-weight: 600;
			color: #000;
		}

		.text-author {
			bordeR: 1px solid rgba(0, 0, 0, .05);
			max-width: 50%;
			margin: 0 auto;
			padding: 2em;
		}

		.text-author img {
			border-radius: 50%;
			padding-bottom: 20px;
		}

		.text-author h3 {
			margin-bottom: 0;
		}

		ul.social {
			padding: 0;
		}

		ul.social li {
			display: inline-block;
			margin-right: 10px;
		}

		/*FOOTER*/
		.footer {
			border-top: 1px solid rgba(0, 0, 0, .05);
			color: rgba(0, 0, 0, .5);
		}

		.footer .heading {
			color: #000;
			font-size: 20px;
		}

		.footer ul {
			margin: 0;
			padding: 0;
		}

		.footer ul li {
			list-style: none;
			margin-bottom: 10px;
		}

		.footer ul li a {
			color: rgba(0, 0, 0, 1);
		}

	</style>
</head>

<body width='100%' style='margin: 0; padding: 0 !important; mso-line-height-rule: exactly; background-color: #f1f1f1;'>
	<center style='width: 100%;'>
		<div
			style='display: none; font-size: 1px;max-height: 0px; max-width: 0px; opacity: 0; overflow: hidden; mso-hide: all; font-family: sans-serif;'>
			&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;
		</div>
		<div style='max-width: 600px; margin: 0 auto;' class='email-container'>
			<!-- BEGIN BODY -->
			<table align='center' role='presentation' cellspacing='0' cellpadding='0' border='0' width='100%'
				style='margin: auto;'>
				<tr>
					<td valign='top' class='bg_white' style='padding: 1em 2.5em 0 2.5em;'>
						<table role='presentation' border='0' cellpadding='0' cellspacing='0' width='100%'>
							<tr>
								<td class='logo' style='text-align: center;'>
									<h1><a href='#'><img src='" + appdata.emailLogoUrl + @"'
												style='width: 250px; height: auto;'></a></h1>
								</td>
							</tr>
						</table>
					</td>
				</tr><!-- end tr -->
				<tr>
					<td valign='middle' class='hero bg_white' style='padding: 2em 0 4em 0;'>
						<table role='presentation' border='0' cellpadding='0' cellspacing='0' width='100%'>
							<tr>
								<td style='padding: 0 2.5em; text-align: center; padding-bottom: 3em;'>
									<div class='text'>
										<h2>Witaj #name #lastname<br /></h2><br>
									</div>
								</td>
							</tr>
							<tr>
								<td style='text-align: center;'>
									<div class='text-author'>
										<h3 class='name'>Twój kod autoryzacyjny do zmiany hasła</h3> <br />
										<h4 class='position'>#token</h4>
									</div>
								</td>
							</tr>
						</table>
					</td>
				</tr><!-- end tr -->
				<!-- 1 Column Text + Button : END -->
			</table>
			<table align='center' role='presentation' cellspacing='0' cellpadding='0' border='0' width='100%'
				style='margin: auto;'>
				<tr>
					<td valign='middle' class='bg_light footer email-section'>
						<table>
							<tr>
								<td valign='top' width='50%' style='padding-top: 20px;'>
									<table role='presentation' cellspacing='0' cellpadding='0' border='0' width='100%'>
										<tr>
											<td style='text-align: left; padding-left: 5px; padding-right: 5px;'>
												<h3 class='heading'>Kontakt:</h3>
												<ul>
													<li><span class='text'>" + appdata.companyName + @".<br />" + appdata.companyAddress + @"<br />" + appdata.companyPostcode + @" " + appdata.companyCity + @"</span></li>
													<li><span class='text'>+48 " + appdata.companyPhone + @"</span></li>
                                                    <li><span class='text'>" + appdata.companyEmail + @"</span></li>
												</ul>
											</td>
										</tr>
									</table>
								</td>
							</tr>
						</table>
					</td>
				</tr><!-- end: tr -->
				<tr> </tr>
			</table>
		</div>
	</center>
</body>

</html>";

        public static string ContactMessageTitle = $"{appdata.companyName} - Nowa wiadomosc";

        public static string ContactMessage = @"<!DOCTYPE html>
<html lang='pl' xmlns='http://www.w3.org/1999/xhtml' xmlns:v='urn:schemas-microsoft-com:vml'
	xmlns:o='urn:schemas-microsoft-com:office:office'>

<head>
	<meta charset='utf-8'> <!-- utf-8 works for most cases -->
	<meta name='viewport' content='width=device-width'> <!-- Forcing initial-scale shouldn't be necessary -->
	<meta http-equiv='X-UA-Compatible' content='IE=edge'> <!-- Use the latest (edge) version of IE rendering engine -->
	<meta name='x-apple-disable-message-reformatting'> <!-- Disable auto-scale in iOS 10 Mail entirely -->
	<title>@Model.Title</title> <!-- The title tag shows in email notifications, like Android 4.4. -->
	<link href='https://fonts.googleapis.com/css?family=Poppins:200,300,400,500,600,700' rel='stylesheet'>
	<!-- CSS Reset : BEGIN -->
	<style>
		/* What it does: Remove spaces around the email design added by some email clients. */
		/* Beware: It can remove the padding / margin and add a background color to the compose a reply window. */
		html,
		body {
			margin: 0 auto !important;
			padding: 0 !important;
			height: 100% !important;
			width: 100% !important;
			background: #f1f1f1;
		}

		/* What it does: Stops email clients resizing small text. */
		* {
			-ms-text-size-adjust: 100%;
			-webkit-text-size-adjust: 100%;
		}

		/* What it does: Centers email on Android 4.4 */
		div[style*='margin: 16px 0'] {
			margin: 0 !important;
		}

		/* What it does: Stops Outlook from adding extra spacing to tables. */
		table,
		td {
			mso-table-lspace: 0pt !important;
			mso-table-rspace: 0pt !important;
		}

		/* What it does: Fixes webkit padding issue. */
		table {
			border-spacing: 0 !important;
			border-collapse: collapse !important;
			table-layout: fixed !important;
			margin: 0 auto !important;
		}

		/* What it does: Uses a better rendering method when resizing images in IE. */
		img {
			-ms-interpolation-mode: bicubic;
		}

		/* What it does: Prevents Windows 10 Mail from underlining links despite inline CSS. Styles for underlined links should be inline. */
		a {
			text-decoration: none;
		}

		/* What it does: A work-around for email clients meddling in triggered links. */
		*[x-apple-data-detectors],
		/* iOS */
		.unstyle-auto-detected-links *,
		.aBn {
			border-bottom: 0 !important;
			cursor: default !important;
			color: inherit !important;
			text-decoration: none !important;
			font-size: inherit !important;
			font-family: inherit !important;
			font-weight: inherit !important;
			line-height: inherit !important;
		}

		/* What it does: Prevents Gmail from displaying a download button on large, non-linked images. */
		.a6S {
			display: none !important;
			opacity: 0.01 !important;
		}

		/* What it does: Prevents Gmail from changing the text color in conversation threads. */
		.im {
			color: inherit !important;
		}

		/* If the above doesn't work, add a .g-img class to any image in question. */
		img.g-img+div {
			display: none !important;
		}

		/* What it does: Removes right gutter in Gmail iOS app: https://github.com/TedGoas/Cerberus/issues/89  */
		/* Create one of these media queries for each additional viewport size you'd like to fix */
		/* iPhone 4, 4S, 5, 5S, 5C, and 5SE */
		@media only screen and (min-device-width: 320px) and (max-device-width: 374px) {
			u~div .email-container {
				min-width: 320px !important;
			}
		}

		/* iPhone 6, 6S, 7, 8, and X */
		@media only screen and (min-device-width: 375px) and (max-device-width: 413px) {
			u~div .email-container {
				min-width: 375px !important;
			}
		}

		/* iPhone 6+, 7+, and 8+ */
		@media only screen and (min-device-width: 414px) {
			u~div .email-container {
				min-width: 414px !important;
			}
		}
	</style> <!-- CSS Reset : END -->
	<style>
		.primary {
			background: #17bebb;
		}

		.bg_white {
			background: #ffffff;
		}

		.bg_light {
			background: #f7fafa;
		}

		.bg_black {
			background: #000000;
		}

		.bg_dark {
			background: rgba(0, 0, 0, .8);
		}

		.email-section {
			padding: 2.5em;
		}

		/*BUTTON*/
		.btn {
			padding: 10px 15px;
			display: inline-block;
		}

		.btn.btn-primary {
			border-radius: 5px;
			background: #17bebb;
			color: #ffffff;
		}

		.btn.btn-white {
			border-radius: 5px;
			background: #ffffff;
			color: #000000;
		}

		.btn.btn-white-outline {
			border-radius: 5px;
			background: transparent;
			border: 1px solid #fff;
			color: #fff;
		}

		.btn.btn-black-outline {
			border-radius: 0px;
			background: transparent;
			border: 2px solid #000;
			color: #000;
			font-weight: 700;
		}

		.btn-custom {
			color: rgba(0, 0, 0, .3);
			text-decoration: underline;
		}

		h1,
		h2,
		h3,
		h4,
		h5,
		h6 {
			font-family: 'Poppins', sans-serif;
			color: #000000;
			margin-top: 0;
			font-weight: 400;
		}

		body {
			font-family: 'Poppins', sans-serif;
			font-weight: 400;
			font-size: 15px;
			line-height: 1.8;
			color: rgba(0, 0, 0, .4);
		}

		a {
			color: #17bebb;
		}

		table {}

		/*LOGO*/
		.logo h1 {
			margin: 0;
		}

		.logo h1 a {
			color: #17bebb;
			font-size: 24px;
			font-weight: 700;
			font-family: 'Poppins', sans-serif;
		}

		/*HERO*/
		.hero {
			position: relative;
			z-index: 0;
		}

		.hero .text {
			color: rgba(0, 0, 0, .3);
		}

		.hero .text h2 {
			color: #000;
			font-size: 34px;
			margin-bottom: 0;
			font-weight: 200;
			line-height: 1.4;
		}

		.hero .text h3 {
			font-size: 24px;
			font-weight: 300;
		}

		.hero .text h2 span {
			font-weight: 600;
			color: #000;
		}

		.text-author {
			bordeR: 1px solid rgba(0, 0, 0, .05);
			max-width: 50%;
			margin: 0 auto;
			padding: 2em;
		}

		.text-author img {
			border-radius: 50%;
			padding-bottom: 20px;
		}

		.text-author h3 {
			margin-bottom: 0;
		}

		ul.social {
			padding: 0;
		}

		ul.social li {
			display: inline-block;
			margin-right: 10px;
		}

		/*FOOTER*/
		.footer {
			border-top: 1px solid rgba(0, 0, 0, .05);
			color: rgba(0, 0, 0, .5);
		}

		.footer .heading {
			color: #000;
			font-size: 20px;
		}

		.footer ul {
			margin: 0;
			padding: 0;
		}

		.footer ul li {
			list-style: none;
			margin-bottom: 10px;
		}

		.footer ul li a {
			color: rgba(0, 0, 0, 1);
		}

		@media screen and (max-width: 500px) {}
	</style> <!-- CSS Reset : END -->
	<!-- Progressive Enhancements : BEGIN -->
	<style>
		.primary {
			background: #17bebb;
		}

		.bg_white {
			background: #ffffff;
		}

		.bg_light {
			background: #f7fafa;
		}

		.bg_black {
			background: #000000;
		}

		.bg_dark {
			background: rgba(0, 0, 0, .8);
		}

		.email-section {
			padding: 2.5em;
		}

		/*BUTTON*/
		.btn {
			padding: 10px 15px;
			display: inline-block;
		}

		.btn.btn-primary {
			border-radius: 5px;
			background: #17bebb;
			color: #ffffff;
		}

		.btn.btn-white {
			border-radius: 5px;
			background: #ffffff;
			color: #000000;
		}

		.btn.btn-white-outline {
			border-radius: 5px;
			background: transparent;
			border: 1px solid #fff;
			color: #fff;
		}

		.btn.btn-black-outline {
			border-radius: 0px;
			background: transparent;
			border: 2px solid #000;
			color: #000;
			font-weight: 700;
		}

		.btn-custom {
			color: rgba(0, 0, 0, .3);
			text-decoration: underline;
		}

		h1,
		h2,
		h3,
		h4,
		h5,
		h6 {
			font-family: 'Poppins', sans-serif;
			color: #000000;
			margin-top: 0;
			font-weight: 400;
		}

		body {
			font-family: 'Poppins', sans-serif;
			font-weight: 400;
			font-size: 15px;
			line-height: 1.8;
			color: rgba(0, 0, 0, .4);
		}

		a {
			color: #17bebb;
		}

		table {}

		/*LOGO*/
		.logo h1 {
			margin: 0;
		}

		.logo h1 a {
			color: #17bebb;
			font-size: 24px;
			font-weight: 700;
			font-family: 'Poppins', sans-serif;
		}

		/*HERO*/
		.hero {
			position: relative;
			z-index: 0;
		}

		.hero .text {
			color: rgba(0, 0, 0, .3);
		}

		.hero .text h2 {
			color: #000;
			font-size: 34px;
			margin-bottom: 0;
			font-weight: 200;
			line-height: 1.4;
		}

		.hero .text h3 {
			font-size: 24px;
			font-weight: 300;
		}

		.hero .text h2 span {
			font-weight: 600;
			color: #000;
		}

		.text-author {
			bordeR: 1px solid rgba(0, 0, 0, .05);
			max-width: 50%;
			margin: 0 auto;
			padding: 2em;
		}

		.text-author img {
			border-radius: 50%;
			padding-bottom: 20px;
		}

		.text-author h3 {
			margin-bottom: 0;
		}

		ul.social {
			padding: 0;
		}

		ul.social li {
			display: inline-block;
			margin-right: 10px;
		}

		/*FOOTER*/
		.footer {
			border-top: 1px solid rgba(0, 0, 0, .05);
			color: rgba(0, 0, 0, .5);
		}

		.footer .heading {
			color: #000;
			font-size: 20px;
		}

		.footer ul {
			margin: 0;
			padding: 0;
		}

		.footer ul li {
			list-style: none;
			margin-bottom: 10px;
		}

		.footer ul li a {
			color: rgba(0, 0, 0, 1);
		}

	</style>
</head>

<body width='100%' style='margin: 0; padding: 0 !important; mso-line-height-rule: exactly; background-color: #f1f1f1;'>
	<center style='width: 100%;'>
		<div
			style='display: none; font-size: 1px;max-height: 0px; max-width: 0px; opacity: 0; overflow: hidden; mso-hide: all; font-family: sans-serif;'>
			&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;
		</div>
		<div style='max-width: 600px; margin: 0 auto;' class='email-container'>
			<!-- BEGIN BODY -->
			<table align='center' role='presentation' cellspacing='0' cellpadding='0' border='0' width='100%'
				style='margin: auto;'>
				<tr>
					<td valign='top' class='bg_white' style='padding: 1em 2.5em 0 2.5em;'>
						<table role='presentation' border='0' cellpadding='0' cellspacing='0' width='100%'>
							<tr>
								<td class='logo' style='text-align: center;'>
									<h1><a href='#'><img src='" + appdata.emailLogoUrl + @"'
												style='width: 250px; height: auto;'></a></h1>
								</td>
							</tr>
						</table>
					</td>
				</tr><!-- end tr -->
				<tr>
					<td valign='middle' class='hero bg_white' style='padding: 2em 0 4em 0;'>
						<table role='presentation' border='0' cellpadding='0' cellspacing='0' width='100%'>
							<tr>
								<td style='padding: 0 2.5em; text-align: center; padding-bottom: 3em;'>
									<div class='text'>
										<h2>Wiadomość od #name #lastname<br />Numer Telefonu: #phone<br/>Email: #email</h2><br>
									</div>
								</td>
							</tr>
							<tr>
								<td style='text-align: center;'>
									<div class='text-author'>
										<h3 class='name'>Treść wiadomości</h3> <br />
										<h4 class='position'>#message</h4>
									</div>
								</td>
							</tr>
						</table>
					</td>
				</tr><!-- end tr -->
				<!-- 1 Column Text + Button : END -->
			</table>
			<table align='center' role='presentation' cellspacing='0' cellpadding='0' border='0' width='100%'
				style='margin: auto;'>
				<tr>
					<td valign='middle' class='bg_light footer email-section'>
						<table>
							<tr>
								<td valign='top' width='50%' style='padding-top: 20px;'>
									<table role='presentation' cellspacing='0' cellpadding='0' border='0' width='100%'>
										<tr>
											<td style='text-align: left; padding-left: 5px; padding-right: 5px;'>
												<h3 class='heading'>Kontakt:</h3>
												<ul>
													<li><span class='text'>" + appdata.companyName + @".<br />" + appdata.companyAddress + @"<br />" + appdata.companyPostcode + @" " + appdata.companyCity + @"</span></li>
													<li><span class='text'>+48 " + appdata.companyPhone + @"</span></li>
                                                    <li><span class='text'>" + appdata.companyEmail + @"</span></li>
												</ul>
											</td>
										</tr>
									</table>
								</td>
							</tr>
						</table>
					</td>
				</tr><!-- end: tr -->
				<tr> </tr>
			</table>
		</div>
	</center>
</body>

</html>";

        public static string AddEstimateTitle = $"{appdata.companyName} nowy kosztorys do twojego zlecenia!";

        public static string AddEstimateMessage = @"<!DOCTYPE html>
<html lang='pl' xmlns='http://www.w3.org/1999/xhtml' xmlns:v='urn:schemas-microsoft-com:vml'
	xmlns:o='urn:schemas-microsoft-com:office:office'>

<head>
	<meta charset='utf-8'> <!-- utf-8 works for most cases -->
	<meta name='viewport' content='width=device-width'> <!-- Forcing initial-scale shouldn't be necessary -->
	<meta http-equiv='X-UA-Compatible' content='IE=edge'> <!-- Use the latest (edge) version of IE rendering engine -->
	<meta name='x-apple-disable-message-reformatting'> <!-- Disable auto-scale in iOS 10 Mail entirely -->
	<title>#title</title> <!-- The title tag shows in email notifications, like Android 4.4. -->
	<link href='https://fonts.googleapis.com/css?family=Poppins:200,300,400,500,600,700' rel='stylesheet'>
	<!-- CSS Reset : BEGIN -->
	<style>
		/* What it does: Remove spaces around the email design added by some email clients. */
		/* Beware: It can remove the padding / margin and add a background color to the compose a reply window. */
		html,
		body {
			margin: 0 auto !important;
			padding: 0 !important;
			height: 100% !important;
			width: 100% !important;
			background: #f1f1f1;
		}

		/* What it does: Stops email clients resizing small text. */
		* {
			-ms-text-size-adjust: 100%;
			-webkit-text-size-adjust: 100%;
		}

		/* What it does: Centers email on Android 4.4 */
		div[style*='margin: 16px 0'] {
			margin: 0 !important;
		}

		/* What it does: Stops Outlook from adding extra spacing to tables. */
		table,
		td {
			mso-table-lspace: 0pt !important;
			mso-table-rspace: 0pt !important;
		}

		/* What it does: Fixes webkit padding issue. */
		table {
			border-spacing: 0 !important;
			border-collapse: collapse !important;
			table-layout: fixed !important;
			margin: 0 auto !important;
		}

		/* What it does: Uses a better rendering method when resizing images in IE. */
		img {
			-ms-interpolation-mode: bicubic;
		}

		/* What it does: Prevents Windows 10 Mail from underlining links despite inline CSS. Styles for underlined links should be inline. */
		a {
			text-decoration: none;
		}

		/* What it does: A work-around for email clients meddling in triggered links. */
		*[x-apple-data-detectors],
		/* iOS */
		.unstyle-auto-detected-links *,
		.aBn {
			border-bottom: 0 !important;
			cursor: default !important;
			color: inherit !important;
			text-decoration: none !important;
			font-size: inherit !important;
			font-family: inherit !important;
			font-weight: inherit !important;
			line-height: inherit !important;
		}

		/* What it does: Prevents Gmail from displaying a download button on large, non-linked images. */
		.a6S {
			display: none !important;
			opacity: 0.01 !important;
		}

		/* What it does: Prevents Gmail from changing the text color in conversation threads. */
		.im {
			color: inherit !important;
		}

		/* If the above doesn't work, add a .g-img class to any image in question. */
		img.g-img+div {
			display: none !important;
		}

		/* What it does: Removes right gutter in Gmail iOS app: https://github.com/TedGoas/Cerberus/issues/89  */
		/* Create one of these media queries for each additional viewport size you'd like to fix */
		/* iPhone 4, 4S, 5, 5S, 5C, and 5SE */
		@media only screen and (min-device-width: 320px) and (max-device-width: 374px) {
			u~div .email-container {
				min-width: 320px !important;
			}
		}

		/* iPhone 6, 6S, 7, 8, and X */
		@media only screen and (min-device-width: 375px) and (max-device-width: 413px) {
			u~div .email-container {
				min-width: 375px !important;
			}
		}

		/* iPhone 6+, 7+, and 8+ */
		@media only screen and (min-device-width: 414px) {
			u~div .email-container {
				min-width: 414px !important;
			}
		}
	</style> <!-- CSS Reset : END -->
	<style>
		.primary {
			background: #17bebb;
		}

		.bg_white {
			background: #ffffff;
		}

		.bg_light {
			background: #f7fafa;
		}

		.bg_black {
			background: #000000;
		}

		.bg_dark {
			background: rgba(0, 0, 0, .8);
		}

		.email-section {
			padding: 2.5em;
		}

		/*BUTTON*/
		.btn {
			padding: 10px 15px;
			display: inline-block;
		}

		.btn.btn-primary {
			border-radius: 5px;
			background: #17bebb;
			color: #ffffff;
		}

		.btn.btn-white {
			border-radius: 5px;
			background: #ffffff;
			color: #000000;
		}

		.btn.btn-white-outline {
			border-radius: 5px;
			background: transparent;
			border: 1px solid #fff;
			color: #fff;
		}

		.btn.btn-black-outline {
			border-radius: 0px;
			background: transparent;
			border: 2px solid #000;
			color: #000;
			font-weight: 700;
		}

		.btn-custom {
			color: rgba(0, 0, 0, .3);
			text-decoration: underline;
		}

		h1,
		h2,
		h3,
		h4,
		h5,
		h6 {
			font-family: 'Poppins', sans-serif;
			color: #000000;
			margin-top: 0;
			font-weight: 400;
		}

		body {
			font-family: 'Poppins', sans-serif;
			font-weight: 400;
			font-size: 15px;
			line-height: 1.8;
			color: rgba(0, 0, 0, .4);
		}

		a {
			color: #17bebb;
		}

		table {}

		/*LOGO*/
		.logo h1 {
			margin: 0;
		}

		.logo h1 a {
			color: #17bebb;
			font-size: 24px;
			font-weight: 700;
			font-family: 'Poppins', sans-serif;
		}

		/*HERO*/
		.hero {
			position: relative;
			z-index: 0;
		}

		.hero .text {
			color: rgba(0, 0, 0, .3);
		}

		.hero .text h2 {
			color: #000;
			font-size: 34px;
			margin-bottom: 0;
			font-weight: 200;
			line-height: 1.4;
		}

		.hero .text h3 {
			font-size: 24px;
			font-weight: 300;
		}

		.hero .text h2 span {
			font-weight: 600;
			color: #000;
		}

		.text-author {
			bordeR: 1px solid rgba(0, 0, 0, .05);
			max-width: 50%;
			margin: 0 auto;
			padding: 2em;
		}

		.text-author img {
			border-radius: 50%;
			padding-bottom: 20px;
		}

		.text-author h3 {
			margin-bottom: 0;
		}

		ul.social {
			padding: 0;
		}

		ul.social li {
			display: inline-block;
			margin-right: 10px;
		}

		/*FOOTER*/
		.footer {
			border-top: 1px solid rgba(0, 0, 0, .05);
			color: rgba(0, 0, 0, .5);
		}

		.footer .heading {
			color: #000;
			font-size: 20px;
		}

		.footer ul {
			margin: 0;
			padding: 0;
		}

		.footer ul li {
			list-style: none;
			margin-bottom: 10px;
		}

		.footer ul li a {
			color: rgba(0, 0, 0, 1);
		}

		@media screen and (max-width: 500px) {}
	</style> <!-- CSS Reset : END -->
	<!-- Progressive Enhancements : BEGIN -->
	<style>
		.primary {
			background: #17bebb;
		}

		.bg_white {
			background: #ffffff;
		}

		.bg_light {
			background: #f7fafa;
		}

		.bg_black {
			background: #000000;
		}

		.bg_dark {
			background: rgba(0, 0, 0, .8);
		}

		.email-section {
			padding: 2.5em;
		}

		/*BUTTON*/
		.btn {
			padding: 10px 15px;
			display: inline-block;
		}

		.btn.btn-primary {
			border-radius: 5px;
			background: #17bebb;
			color: #ffffff;
		}

		.btn.btn-white {
			border-radius: 5px;
			background: #ffffff;
			color: #000000;
		}

		.btn.btn-white-outline {
			border-radius: 5px;
			background: transparent;
			border: 1px solid #fff;
			color: #fff;
		}

		.btn.btn-black-outline {
			border-radius: 0px;
			background: transparent;
			border: 2px solid #000;
			color: #000;
			font-weight: 700;
		}

		.btn-custom {
			color: rgba(0, 0, 0, .3);
			text-decoration: underline;
		}

		h1,
		h2,
		h3,
		h4,
		h5,
		h6 {
			font-family: 'Poppins', sans-serif;
			color: #000000;
			margin-top: 0;
			font-weight: 400;
		}

		body {
			font-family: 'Poppins', sans-serif;
			font-weight: 400;
			font-size: 15px;
			line-height: 1.8;
			color: rgba(0, 0, 0, .4);
		}

		a {
			color: #17bebb;
		}

		table {}

		/*LOGO*/
		.logo h1 {
			margin: 0;
		}

		.logo h1 a {
			color: #17bebb;
			font-size: 24px;
			font-weight: 700;
			font-family: 'Poppins', sans-serif;
		}

		/*HERO*/
		.hero {
			position: relative;
			z-index: 0;
		}

		.hero .text {
			color: rgba(0, 0, 0, .3);
		}

		.hero .text h2 {
			color: #000;
			font-size: 34px;
			margin-bottom: 0;
			font-weight: 200;
			line-height: 1.4;
		}

		.hero .text h3 {
			font-size: 24px;
			font-weight: 300;
		}

		.hero .text h2 span {
			font-weight: 600;
			color: #000;
		}

		.text-author {
			bordeR: 1px solid rgba(0, 0, 0, .05);
			max-width: 50%;
			margin: 0 auto;
			padding: 2em;
		}

		.text-author img {
			border-radius: 50%;
			padding-bottom: 20px;
		}

		.text-author h3 {
			margin-bottom: 0;
		}

		ul.social {
			padding: 0;
		}

		ul.social li {
			display: inline-block;
			margin-right: 10px;
		}

		/*FOOTER*/
		.footer {
			border-top: 1px solid rgba(0, 0, 0, .05);
			color: rgba(0, 0, 0, .5);
		}

		.footer .heading {
			color: #000;
			font-size: 20px;
		}

		.footer ul {
			margin: 0;
			padding: 0;
		}

		.footer ul li {
			list-style: none;
			margin-bottom: 10px;
		}

		.footer ul li a {
			color: rgba(0, 0, 0, 1);
		}

		.vehicle-box {
    display: flex;
    justify-content: center;
    align-items: center;
    gap: 0.625rem;
}

    .vehicle-box .medapp-text-table-secondary {
        margin-top: 6px;
    }

.vehicle-row {
    display: flex;
    width: 100%;
    align-items: center;
    justify-content: flex-start;
}

.user-vehicle-box {
    height: 5.25rem;
    justify-content: flex-start;
    transition: height 0.5s ease;
}

.vehicle-table {
    width: 100%;
}

.orders-cost-elem {
    display: flex;
    width: 100%;
    flex-direction: column;
    justify-content: center;
    align-items: center;
    border-bottom: 1px solid #2C82E0;
}

.paragraph-bold {
        font-family: 'Source Sans Pro';
        font-style: normal;
        font-weight: 700;
        font-size: 1rem;
        line-height: 1.5rem;
        /* identical to box height, or 150% */


        color: #000000;
    }

    .paragraph {
        font-family: 'Source Sans Pro';
    font-style: normal;
    font-weight: 400;
    font-size: 1rem;
    line-height: 1.5rem;
    /* identical to box height, or 150% */


    color: #000000;
    }

	</style>
</head>

<body width='100%' style='margin: 0; padding: 0 !important; mso-line-height-rule: exactly; background-color: #f1f1f1;'>
	<center style='width: 100%;'>
		<div
			style='display: none; font-size: 1px;max-height: 0px; max-width: 0px; opacity: 0; overflow: hidden; mso-hide: all; font-family: sans-serif;'>
			&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;
		</div>
		<div style='max-width: 600px; margin: 0 auto;' class='email-container'>
			<!-- BEGIN BODY -->
			<table align='center' role='presentation' cellspacing='0' cellpadding='0' border='0' width='100%'
				style='margin: auto;'>
				<tr>
					<td valign='top' class='bg_white' style='padding: 1em 2.5em 0 2.5em;'>
						<table role='presentation' border='0' cellpadding='0' cellspacing='0' width='100%'>
							<tr>
								<td class='logo' style='text-align: center;'>
									<h1><a href='#'><img src='" + appdata.emailLogoUrl + @"'
												style='width: 250px; height: auto;'></a></h1>
								</td>
							</tr>
						</table>
					</td>
				</tr><!-- end tr -->
				<tr>
					<td valign='middle' class='hero bg_white' style='padding: 2em 0 4em 0;'>
						<table role='presentation' border='0' cellpadding='0' cellspacing='0' width='100%'>
							<tr>
								<td style='padding: 0 2.5em; text-align: center; padding-bottom: 3em;'>
									<div class='text'>
										<h2>Witaj #name #lastname<br /></h2><br>
										
								</td>
							</tr>
							<tr>
								<td style='text-align: center;'>
									<div class='text-author' style=""width: 75%; max-width: 75%;"">
										<h3 class='name'>Do twojego zlecenia: #orderNumber został stworzony kosztorys.</h3> <br />
										<div style='display: flex; justify-content: flex-start; align-items: flex-start; width: 100%; gap: 0.313rem; flex-direction: column; margin-bottom: 1.5rem;'>
											<span class='paragraph-bold'>Samochód:</span><br>
											<span class='paragraph'>#vehicleProducer&nbsp;#vehicleModel</span><br>
											<span class='paragraph'>Numer rejestracyjny:&nbsp; #registerNumber</span>
											</div>
											<div style='display: flex; justify-content: center; align-items: center; width: 100%; height: auto; margin-bottom: 2rem;'>
												<span class='paragraph-bold' style='font-size: 2rem;'>Kosztorys</span>
											</div>
											<div style='display: flex; justify-content: center; align-items: center; width: 100%; height: auto; margin-bottom: 1.5rem;'>
												<span class='paragraph-bold' style='font-size: 2rem;'>Części:</span>
											</div>
											<div style='display: flex; justify-content: flex-start; align-items: center; flex-direction: column; width: 100%; gap: 0.625rem; margin-bottom: 2rem;'>
												#parts
											</div>
											<div style='display: flex; justify-content: center; align-items: center; width: 100%; height: auto; margin-bottom: 1.5rem;'>
												<span class='paragraph-bold' style='font-size: 2rem;'>Usługi:</span>
											</div>
											<div style='display: flex; justify-content: flex-start; align-items: flex-start; flex-direction: column; width: 100%; gap: 0.625rem; margin-bottom: 3rem;' >
												#services
											</div>
											<div class='orders-cost-elem'>
												<div class='vehicle-row' style='justify-content: flex-end; gap: 2rem;'>
													<span class='paragraph-bold'>Razem części:&nbsp;</span>
													<span class='paragraph' id='costscheme-total-price-parts'>#totalPartsPrice PLN BRUTTO</span>
												</div>
												<div class='vehicle-row' style='justify-content: flex-end; gap: 2rem;'>
													<span class='paragraph-bold'>Razem usługi:&nbsp;</span>
													<span class='paragraph' id='costcheme-total-price-services'>#totalServicesPrice PLN BRUTTO</span>
												</div>
											</div>
											<div class='vehicle-row' style='justify-content: flex-end; gap: 2rem;'>
												<span class='paragraph-bold'>SUMA:&nbsp;</span>
												<span class='paragraph' id='costscheme-total-price'>#totalPrice PLN BRUTTO</span>
											</div>
									</div>
										<h4 class='name'>Możesz go zaakceptować w portalu klienta lub dzwoniąc na numer: 504 007 024</h4>
										<p><a href='" + appdata.loginUrl + @" class='btn btn-primary'>Przejdź do logowania</a></p>
									</div>
								</td>
							</tr>
						</table>
					</td>
				</tr><!-- end tr -->
				<!-- 1 Column Text + Button : END -->
			</table>
			<table align='center' role='presentation' cellspacing='0' cellpadding='0' border='0' width='100%'
				style='margin: auto;'>
				<tr>
					<td valign='middle' class='bg_light footer email-section'>
						<table>
							<tr>
								<td valign='top' width='50%' style='padding-top: 20px;'>
									<table role='presentation' cellspacing='0' cellpadding='0' border='0' width='100%'>
										<tr>
											<td style='text-align: left; padding-left: 5px; padding-right: 5px;'>
												<h3 class='heading'>Kontakt:</h3>
												<ul>
													<li><span class='text'>" + appdata.companyName + @".<br />" + appdata.companyAddress + @"<br />" + appdata.companyPostcode + @" " + appdata.companyCity + @"</span></li>
													<li><span class='text'>+48 " + appdata.companyPhone + @"</span></li>
                                                    <li><span class='text'>" + appdata.companyEmail + @"</span></li>
												</ul>
											</td>
										</tr>
									</table>
								</td>
							</tr>
						</table>
					</td>
				</tr><!-- end: tr -->
				<tr> </tr>
			</table>
		</div>
	</center>
</body>

</html>";

        public static string EditEstimateMessageTitle = $"{appdata.companyName} - Edycja kosztorysu w zleceniu naprawy!";

        public static string EditEstimateMessage = @"<!DOCTYPE html>
<html lang='pl' xmlns='http://www.w3.org/1999/xhtml' xmlns:v='urn:schemas-microsoft-com:vml'
	xmlns:o='urn:schemas-microsoft-com:office:office'>

<head>
	<meta charset='utf-8'> <!-- utf-8 works for most cases -->
	<meta name='viewport' content='width=device-width'> <!-- Forcing initial-scale shouldn't be necessary -->
	<meta http-equiv='X-UA-Compatible' content='IE=edge'> <!-- Use the latest (edge) version of IE rendering engine -->
	<meta name='x-apple-disable-message-reformatting'> <!-- Disable auto-scale in iOS 10 Mail entirely -->
	<title>#title</title> <!-- The title tag shows in email notifications, like Android 4.4. -->
	<link href='https://fonts.googleapis.com/css?family=Poppins:200,300,400,500,600,700' rel='stylesheet'>
	<!-- CSS Reset : BEGIN -->
	<style>
		/* What it does: Remove spaces around the email design added by some email clients. */
		/* Beware: It can remove the padding / margin and add a background color to the compose a reply window. */
		html,
		body {
			margin: 0 auto !important;
			padding: 0 !important;
			height: 100% !important;
			width: 100% !important;
			background: #f1f1f1;
		}

		/* What it does: Stops email clients resizing small text. */
		* {
			-ms-text-size-adjust: 100%;
			-webkit-text-size-adjust: 100%;
		}

		/* What it does: Centers email on Android 4.4 */
		div[style*='margin: 16px 0'] {
			margin: 0 !important;
		}

		/* What it does: Stops Outlook from adding extra spacing to tables. */
		table,
		td {
			mso-table-lspace: 0pt !important;
			mso-table-rspace: 0pt !important;
		}

		/* What it does: Fixes webkit padding issue. */
		table {
			border-spacing: 0 !important;
			border-collapse: collapse !important;
			table-layout: fixed !important;
			margin: 0 auto !important;
		}

		/* What it does: Uses a better rendering method when resizing images in IE. */
		img {
			-ms-interpolation-mode: bicubic;
		}

		/* What it does: Prevents Windows 10 Mail from underlining links despite inline CSS. Styles for underlined links should be inline. */
		a {
			text-decoration: none;
		}

		/* What it does: A work-around for email clients meddling in triggered links. */
		*[x-apple-data-detectors],
		/* iOS */
		.unstyle-auto-detected-links *,
		.aBn {
			border-bottom: 0 !important;
			cursor: default !important;
			color: inherit !important;
			text-decoration: none !important;
			font-size: inherit !important;
			font-family: inherit !important;
			font-weight: inherit !important;
			line-height: inherit !important;
		}

		/* What it does: Prevents Gmail from displaying a download button on large, non-linked images. */
		.a6S {
			display: none !important;
			opacity: 0.01 !important;
		}

		/* What it does: Prevents Gmail from changing the text color in conversation threads. */
		.im {
			color: inherit !important;
		}

		/* If the above doesn't work, add a .g-img class to any image in question. */
		img.g-img+div {
			display: none !important;
		}

		/* What it does: Removes right gutter in Gmail iOS app: https://github.com/TedGoas/Cerberus/issues/89  */
		/* Create one of these media queries for each additional viewport size you'd like to fix */
		/* iPhone 4, 4S, 5, 5S, 5C, and 5SE */
		@media only screen and (min-device-width: 320px) and (max-device-width: 374px) {
			u~div .email-container {
				min-width: 320px !important;
			}
		}

		/* iPhone 6, 6S, 7, 8, and X */
		@media only screen and (min-device-width: 375px) and (max-device-width: 413px) {
			u~div .email-container {
				min-width: 375px !important;
			}
		}

		/* iPhone 6+, 7+, and 8+ */
		@media only screen and (min-device-width: 414px) {
			u~div .email-container {
				min-width: 414px !important;
			}
		}
	</style> <!-- CSS Reset : END -->
	<style>
		.primary {
			background: #17bebb;
		}

		.bg_white {
			background: #ffffff;
		}

		.bg_light {
			background: #f7fafa;
		}

		.bg_black {
			background: #000000;
		}

		.bg_dark {
			background: rgba(0, 0, 0, .8);
		}

		.email-section {
			padding: 2.5em;
		}

		/*BUTTON*/
		.btn {
			padding: 10px 15px;
			display: inline-block;
		}

		.btn.btn-primary {
			border-radius: 5px;
			background: #17bebb;
			color: #ffffff;
		}

		.btn.btn-white {
			border-radius: 5px;
			background: #ffffff;
			color: #000000;
		}

		.btn.btn-white-outline {
			border-radius: 5px;
			background: transparent;
			border: 1px solid #fff;
			color: #fff;
		}

		.btn.btn-black-outline {
			border-radius: 0px;
			background: transparent;
			border: 2px solid #000;
			color: #000;
			font-weight: 700;
		}

		.btn-custom {
			color: rgba(0, 0, 0, .3);
			text-decoration: underline;
		}

		h1,
		h2,
		h3,
		h4,
		h5,
		h6 {
			font-family: 'Poppins', sans-serif;
			color: #000000;
			margin-top: 0;
			font-weight: 400;
		}

		body {
			font-family: 'Poppins', sans-serif;
			font-weight: 400;
			font-size: 15px;
			line-height: 1.8;
			color: rgba(0, 0, 0, .4);
		}

		a {
			color: #17bebb;
		}

		table {}

		/*LOGO*/
		.logo h1 {
			margin: 0;
		}

		.logo h1 a {
			color: #17bebb;
			font-size: 24px;
			font-weight: 700;
			font-family: 'Poppins', sans-serif;
		}

		/*HERO*/
		.hero {
			position: relative;
			z-index: 0;
		}

		.hero .text {
			color: rgba(0, 0, 0, .3);
		}

		.hero .text h2 {
			color: #000;
			font-size: 34px;
			margin-bottom: 0;
			font-weight: 200;
			line-height: 1.4;
		}

		.hero .text h3 {
			font-size: 24px;
			font-weight: 300;
		}

		.hero .text h2 span {
			font-weight: 600;
			color: #000;
		}

		.text-author {
			bordeR: 1px solid rgba(0, 0, 0, .05);
			max-width: 50%;
			margin: 0 auto;
			padding: 2em;
		}

		.text-author img {
			border-radius: 50%;
			padding-bottom: 20px;
		}

		.text-author h3 {
			margin-bottom: 0;
		}

		ul.social {
			padding: 0;
		}

		ul.social li {
			display: inline-block;
			margin-right: 10px;
		}

		/*FOOTER*/
		.footer {
			border-top: 1px solid rgba(0, 0, 0, .05);
			color: rgba(0, 0, 0, .5);
		}

		.footer .heading {
			color: #000;
			font-size: 20px;
		}

		.footer ul {
			margin: 0;
			padding: 0;
		}

		.footer ul li {
			list-style: none;
			margin-bottom: 10px;
		}

		.footer ul li a {
			color: rgba(0, 0, 0, 1);
		}

		@media screen and (max-width: 500px) {}
	</style> <!-- CSS Reset : END -->
	<!-- Progressive Enhancements : BEGIN -->
	<style>
		.primary {
			background: #17bebb;
		}

		.bg_white {
			background: #ffffff;
		}

		.bg_light {
			background: #f7fafa;
		}

		.bg_black {
			background: #000000;
		}

		.bg_dark {
			background: rgba(0, 0, 0, .8);
		}

		.email-section {
			padding: 2.5em;
		}

		/*BUTTON*/
		.btn {
			padding: 10px 15px;
			display: inline-block;
		}

		.btn.btn-primary {
			border-radius: 5px;
			background: #17bebb;
			color: #ffffff;
		}

		.btn.btn-white {
			border-radius: 5px;
			background: #ffffff;
			color: #000000;
		}

		.btn.btn-white-outline {
			border-radius: 5px;
			background: transparent;
			border: 1px solid #fff;
			color: #fff;
		}

		.btn.btn-black-outline {
			border-radius: 0px;
			background: transparent;
			border: 2px solid #000;
			color: #000;
			font-weight: 700;
		}

		.btn-custom {
			color: rgba(0, 0, 0, .3);
			text-decoration: underline;
		}

		h1,
		h2,
		h3,
		h4,
		h5,
		h6 {
			font-family: 'Poppins', sans-serif;
			color: #000000;
			margin-top: 0;
			font-weight: 400;
		}

		body {
			font-family: 'Poppins', sans-serif;
			font-weight: 400;
			font-size: 15px;
			line-height: 1.8;
			color: rgba(0, 0, 0, .4);
		}

		a {
			color: #17bebb;
		}

		table {}

		/*LOGO*/
		.logo h1 {
			margin: 0;
		}

		.logo h1 a {
			color: #17bebb;
			font-size: 24px;
			font-weight: 700;
			font-family: 'Poppins', sans-serif;
		}

		/*HERO*/
		.hero {
			position: relative;
			z-index: 0;
		}

		.hero .text {
			color: rgba(0, 0, 0, .3);
		}

		.hero .text h2 {
			color: #000;
			font-size: 34px;
			margin-bottom: 0;
			font-weight: 200;
			line-height: 1.4;
		}

		.hero .text h3 {
			font-size: 24px;
			font-weight: 300;
		}

		.hero .text h2 span {
			font-weight: 600;
			color: #000;
		}

		.text-author {
			bordeR: 1px solid rgba(0, 0, 0, .05);
			max-width: 50%;
			margin: 0 auto;
			padding: 2em;
		}

		.text-author img {
			border-radius: 50%;
			padding-bottom: 20px;
		}

		.text-author h3 {
			margin-bottom: 0;
		}

		ul.social {
			padding: 0;
		}

		ul.social li {
			display: inline-block;
			margin-right: 10px;
		}

		/*FOOTER*/
		.footer {
			border-top: 1px solid rgba(0, 0, 0, .05);
			color: rgba(0, 0, 0, .5);
		}

		.footer .heading {
			color: #000;
			font-size: 20px;
		}

		.footer ul {
			margin: 0;
			padding: 0;
		}

		.footer ul li {
			list-style: none;
			margin-bottom: 10px;
		}

		.footer ul li a {
			color: rgba(0, 0, 0, 1);
		}

		.vehicle-box {
    display: flex;
    justify-content: center;
    align-items: center;
    gap: 0.625rem;
}

    .vehicle-box .medapp-text-table-secondary {
        margin-top: 6px;
    }

.vehicle-row {
    display: flex;
    width: 100%;
    align-items: center;
    justify-content: flex-start;
}

.user-vehicle-box {
    height: 5.25rem;
    justify-content: flex-start;
    transition: height 0.5s ease;
}

.vehicle-table {
    width: 100%;
}

.orders-cost-elem {
    display: flex;
    width: 100%;
    flex-direction: column;
    justify-content: center;
    align-items: center;
    border-bottom: 1px solid #2C82E0;
}

.paragraph-bold {
        font-family: 'Source Sans Pro';
        font-style: normal;
        font-weight: 700;
        font-size: 1rem;
        line-height: 1.5rem;
        /* identical to box height, or 150% */


        color: #000000;
    }

    .paragraph {
        font-family: 'Source Sans Pro';
    font-style: normal;
    font-weight: 400;
    font-size: 1rem;
    line-height: 1.5rem;
    /* identical to box height, or 150% */


    color: #000000;
    }

	</style>
</head>

<body width='100%' style='margin: 0; padding: 0 !important; mso-line-height-rule: exactly; background-color: #f1f1f1;'>
	<center style='width: 100%;'>
		<div
			style='display: none; font-size: 1px;max-height: 0px; max-width: 0px; opacity: 0; overflow: hidden; mso-hide: all; font-family: sans-serif;'>
			&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;
		</div>
		<div style='max-width: 600px; margin: 0 auto;' class='email-container'>
			<!-- BEGIN BODY -->
			<table align='center' role='presentation' cellspacing='0' cellpadding='0' border='0' width='100%'
				style='margin: auto;'>
				<tr>
					<td valign='top' class='bg_white' style='padding: 1em 2.5em 0 2.5em;'>
						<table role='presentation' border='0' cellpadding='0' cellspacing='0' width='100%'>
							<tr>
								<td class='logo' style='text-align: center;'>
									<h1><a href='#'><img src='" + appdata.emailLogoUrl + @"'
												style='width: 250px; height: auto;'></a></h1>
								</td>
							</tr>
						</table>
					</td>
				</tr><!-- end tr -->
				<tr>
					<td valign='middle' class='hero bg_white' style='padding: 2em 0 4em 0;'>
						<table role='presentation' border='0' cellpadding='0' cellspacing='0' width='100%'>
							<tr>
								<td style='padding: 0 2.5em; text-align: center; padding-bottom: 3em;'>
									<div class='text'>
										<h2>Witaj #name @lastname<br /></h2><br>
										
								</td>
							</tr>
							<tr>
								<td style='text-align: center;'>
									<div class='text-author' style=""width: 75%; max-width: 75%;"">
										<h3 class='name'>W twoim zleceniu: #orderNumber został zmieniony kosztorys.</h3> <br />
										<div style='display: flex; justify-content: flex-start; align-items: flex-start; width: 100%; gap: 0.313rem; flex-direction: column; margin-bottom: 1.5rem;'>
											<span class='paragraph-bold'>Samochód:</span>
											<span class='paragraph'>#vehicleProducer #vehicleModel</span>
											<span class='paragraph'>Numer rejestracyjny: #registerNumber</span>
											</div>
											<div style='display: flex; justify-content: center; align-items: center; width: 100%; height: auto; margin-bottom: 2rem;'>
												<span class='paragraph-bold' style='font-size: 2rem;'>Kosztorys</span>
											</div>
											<div style='display: flex; justify-content: center; align-items: center; width: 100%; height: auto; margin-bottom: 1.5rem;'>
												<span class='paragraph-bold' style='font-size: 2rem;'>&nbspCzęści:</span>
											</div>
											<div style='display: flex; justify-content: flex-start; align-items: center; flex-direction: column; width: 100%; gap: 0.625rem; margin-bottom: 2rem;'>
												#parts
											</div>
											<div style='display: flex; justify-content: center; align-items: center; width: 100%; height: auto; margin-bottom: 1.5rem;'>
												<span class='paragraph-bold' style='font-size: 2rem;'>Usługi:</span>
											</div>
											<div style='display: flex; justify-content: flex-start; align-items: flex-start; flex-direction: column; width: 100%; gap: 0.625rem; margin-bottom: 3rem;' >
												#services
											</div>
											<div class='orders-cost-elem'>
												<div class='vehicle-row' style='justify-content: flex-end; gap: 2rem;'>
													<span class='paragraph-bold'>Razem części:</span>
													<span class='paragraph' id='costscheme-total-price-parts'>#totalPartsPrice PLN BRUTTO</span>
												</div>
												<div class='vehicle-row' style='justify-content: flex-end; gap: 2rem;'>
													<span class='paragraph-bold'>Razem usługi:</span>
													<span class='paragraph' id='costcheme-total-price-services'>#totalServicesPrice PLN BRUTTO</span>
												</div>
											</div>
											<div class='vehicle-row' style='justify-content: flex-end; gap: 2rem;'>
												<span class='paragraph-bold'>SUMA:</span>
												<span class='paragraph' id='costscheme-total-price'>#totalPrice PLN BRUTTO</span>
											</div>
									</div>
										<h4 class='name'>Możesz zaakceptować zmiany w portalu klienta lub dzwoniąc na numer: 504 007 024</h4>
										<p><a href='" + appdata.loginUrl + @"' class='btn btn-primary'>Przejdź do logowania</a></p>
									</div>
								</td>
							</tr>
						</table>
					</td>
				</tr><!-- end tr -->
				<!-- 1 Column Text + Button : END -->
			</table>
			<table align='center' role='presentation' cellspacing='0' cellpadding='0' border='0' width='100%'
				style='margin: auto;'>
				<tr>
					<td valign='middle' class='bg_light footer email-section'>
						<table align='center' role='presentation' cellspacing='0' cellpadding='0' border='0' width='100%'
				style='margin: auto;'>
				<tr>
					<td valign='middle' class='bg_light footer email-section'>
						<table>
							<tr>
								<td valign='top' width='50%' style='padding-top: 20px;'>
									<table role='presentation' cellspacing='0' cellpadding='0' border='0' width='100%'>
										<tr>
											<td style='text-align: left; padding-left: 5px; padding-right: 5px;'>
												<h3 class='heading'>Kontakt:</h3>
												<ul>
													<li><span class='text'>" + appdata.companyName + @".<br />" + appdata.companyAddress + @"<br />" + appdata.companyPostcode + @" " + appdata.companyCity + @"</span></li>
													<li><span class='text'>+48 " + appdata.companyPhone + @"</span></li>
                                                    <li><span class='text'>" + appdata.companyEmail + @"</span></li>
												</ul>
											</td>
										</tr>
									</table>
								</td>
							</tr>
						</table>
					</td>
				</tr><!-- end: tr -->
				<tr> </tr>
			</table>
					</td>
				</tr><!-- end: tr -->
				<tr> </tr>
			</table>
		</div>
	</center>
</body>

</html>";

        public static string OrderStatusReadyMessageTitle = $"{appdata.companyName} - Samochod gotowy do odbioru!";

        public static string OrderStatusReadyMessage = @"<!DOCTYPE html>
<html lang='pl' xmlns='http://www.w3.org/1999/xhtml' xmlns:v='urn:schemas-microsoft-com:vml'
	xmlns:o='urn:schemas-microsoft-com:office:office'>

<head>
	<meta charset='utf-8'> <!-- utf-8 works for most cases -->
	<meta name='viewport' content='width=device-width'> <!-- Forcing initial-scale shouldn't be necessary -->
	<meta http-equiv='X-UA-Compatible' content='IE=edge'> <!-- Use the latest (edge) version of IE rendering engine -->
	<meta name='x-apple-disable-message-reformatting'> <!-- Disable auto-scale in iOS 10 Mail entirely -->
	<title>#title</title> <!-- The title tag shows in email notifications, like Android 4.4. -->
	<link href='https://fonts.googleapis.com/css?family=Poppins:200,300,400,500,600,700' rel='stylesheet'>
	<!-- CSS Reset : BEGIN -->
	<style>
		/* What it does: Remove spaces around the email design added by some email clients. */
		/* Beware: It can remove the padding / margin and add a background color to the compose a reply window. */
		html,
		body {
			margin: 0 auto !important;
			padding: 0 !important;
			height: 100% !important;
			width: 100% !important;
			background: #f1f1f1;
		}

		/* What it does: Stops email clients resizing small text. */
		* {
			-ms-text-size-adjust: 100%;
			-webkit-text-size-adjust: 100%;
		}

		/* What it does: Centers email on Android 4.4 */
		div[style*='margin: 16px 0'] {
			margin: 0 !important;
		}

		/* What it does: Stops Outlook from adding extra spacing to tables. */
		table,
		td {
			mso-table-lspace: 0pt !important;
			mso-table-rspace: 0pt !important;
		}

		/* What it does: Fixes webkit padding issue. */
		table {
			border-spacing: 0 !important;
			border-collapse: collapse !important;
			table-layout: fixed !important;
			margin: 0 auto !important;
		}

		/* What it does: Uses a better rendering method when resizing images in IE. */
		img {
			-ms-interpolation-mode: bicubic;
		}

		/* What it does: Prevents Windows 10 Mail from underlining links despite inline CSS. Styles for underlined links should be inline. */
		a {
			text-decoration: none;
		}

		/* What it does: A work-around for email clients meddling in triggered links. */
		*[x-apple-data-detectors],
		/* iOS */
		.unstyle-auto-detected-links *,
		.aBn {
			border-bottom: 0 !important;
			cursor: default !important;
			color: inherit !important;
			text-decoration: none !important;
			font-size: inherit !important;
			font-family: inherit !important;
			font-weight: inherit !important;
			line-height: inherit !important;
		}

		/* What it does: Prevents Gmail from displaying a download button on large, non-linked images. */
		.a6S {
			display: none !important;
			opacity: 0.01 !important;
		}

		/* What it does: Prevents Gmail from changing the text color in conversation threads. */
		.im {
			color: inherit !important;
		}

		/* If the above doesn't work, add a .g-img class to any image in question. */
		img.g-img+div {
			display: none !important;
		}

		/* What it does: Removes right gutter in Gmail iOS app: https://github.com/TedGoas/Cerberus/issues/89  */
		/* Create one of these media queries for each additional viewport size you'd like to fix */
		/* iPhone 4, 4S, 5, 5S, 5C, and 5SE */
		@media only screen and (min-device-width: 320px) and (max-device-width: 374px) {
			u~div .email-container {
				min-width: 320px !important;
			}
		}

		/* iPhone 6, 6S, 7, 8, and X */
		@media only screen and (min-device-width: 375px) and (max-device-width: 413px) {
			u~div .email-container {
				min-width: 375px !important;
			}
		}

		/* iPhone 6+, 7+, and 8+ */
		@media only screen and (min-device-width: 414px) {
			u~div .email-container {
				min-width: 414px !important;
			}
		}
	</style> <!-- CSS Reset : END -->
	<style>
	.primary {
			background: #17bebb;
		}

		.bg_white {
			background: #ffffff;
		}

		.bg_light {
			background: #f7fafa;
		}

		.bg_black {
			background: #000000;
		}

		.bg_dark {
			background: rgba(0, 0, 0, .8);
		}

		.email-section {
			padding: 2.5em;
		}

		/*BUTTON*/
		.btn {
			padding: 10px 15px;
			display: inline-block;
		}

		.btn.btn-primary {
			border-radius: 5px;
			background: #17bebb;
			color: #ffffff;
		}

		.btn.btn-white {
			border-radius: 5px;
			background: #ffffff;
			color: #000000;
		}

		.btn.btn-white-outline {
			border-radius: 5px;
			background: transparent;
			border: 1px solid #fff;
			color: #fff;
		}

		.btn.btn-black-outline {
			border-radius: 0px;
			background: transparent;
			border: 2px solid #000;
			color: #000;
			font-weight: 700;
		}

		.btn-custom {
			color: rgba(0, 0, 0, .3);
			text-decoration: underline;
		}

		h1,
		h2,
		h3,
		h4,
		h5,
		h6 {
			font-family: 'Poppins', sans-serif;
			color: #000000;
			margin-top: 0;
			font-weight: 400;
		}

		body {
			font-family: 'Poppins', sans-serif;
			font-weight: 400;
			font-size: 15px;
			line-height: 1.8;
			color: rgba(0, 0, 0, .4);
		}

		a {
			color: #17bebb;
		}

		table {}

		/*LOGO*/
		.logo h1 {
			margin: 0;
		}

		.logo h1 a {
			color: #17bebb;
			font-size: 24px;
			font-weight: 700;
			font-family: 'Poppins', sans-serif;
		}

		/*HERO*/
		.hero {
			position: relative;
			z-index: 0;
		}

		.hero .text {
			color: rgba(0, 0, 0, .3);
		}

		.hero .text h2 {
			color: #000;
			font-size: 34px;
			margin-bottom: 0;
			font-weight: 200;
			line-height: 1.4;
		}

		.hero .text h3 {
			font-size: 24px;
			font-weight: 300;
		}

		.hero .text h2 span {
			font-weight: 600;
			color: #000;
		}

		.text-author {
			bordeR: 1px solid rgba(0, 0, 0, .05);
			max-width: 50%;
			margin: 0 auto;
			padding: 2em;
		}

		.text-author img {
			border-radius: 50%;
			padding-bottom: 20px;
		}

		.text-author h3 {
			margin-bottom: 0;
		}

		ul.social {
			padding: 0;
		}

		ul.social li {
			display: inline-block;
			margin-right: 10px;
		}

		/*FOOTER*/
		.footer {
			border-top: 1px solid rgba(0, 0, 0, .05);
			color: rgba(0, 0, 0, .5);
		}

		.footer .heading {
			color: #000;
			font-size: 20px;
		}

		.footer ul {
			margin: 0;
			padding: 0;
		}

		.footer ul li {
			list-style: none;
			margin-bottom: 10px;
		}

		.footer ul li a {
			color: rgba(0, 0, 0, 1);
		}

		@media screen and (max-width: 500px) {}
	</style> <!-- CSS Reset : END -->
	<!-- Progressive Enhancements : BEGIN -->
	<style>
		.primary {
			background: #17bebb;
		}

		.bg_white {
			background: #ffffff;
		}

		.bg_light {
			background: #f7fafa;
		}

		.bg_black {
			background: #000000;
		}

		.bg_dark {
			background: rgba(0, 0, 0, .8);
		}

		.email-section {
			padding: 2.5em;
		}

		/*BUTTON*/
		.btn {
			padding: 10px 15px;
			display: inline-block;
		}

		.btn.btn-primary {
			border-radius: 5px;
			background: #17bebb;
			color: #ffffff;
		}

		.btn.btn-white {
			border-radius: 5px;
			background: #ffffff;
			color: #000000;
		}

		.btn.btn-white-outline {
			border-radius: 5px;
			background: transparent;
			border: 1px solid #fff;
			color: #fff;
		}

		.btn.btn-black-outline {
			border-radius: 0px;
			background: transparent;
			border: 2px solid #000;
			color: #000;
			font-weight: 700;
		}

		.btn-custom {
			color: rgba(0, 0, 0, .3);
			text-decoration: underline;
		}

		h1,
		h2,
		h3,
		h4,
		h5,
		h6 {
			font-family: 'Poppins', sans-serif;
			color: #000000;
			margin-top: 0;
			font-weight: 400;
		}

		body {
			font-family: 'Poppins', sans-serif;
			font-weight: 400;
			font-size: 15px;
			line-height: 1.8;
			color: rgba(0, 0, 0, .4);
		}

		a {
			color: #17bebb;
		}

		table {}

		/*LOGO*/
		.logo h1 {
			margin: 0;
		}

		.logo h1 a {
			color: #17bebb;
			font-size: 24px;
			font-weight: 700;
			font-family: 'Poppins', sans-serif;
		}

		/*HERO*/
		.hero {
			position: relative;
			z-index: 0;
		}

		.hero .text {
			color: rgba(0, 0, 0, .3);
		}

		.hero .text h2 {
			color: #000;
			font-size: 34px;
			margin-bottom: 0;
			font-weight: 200;
			line-height: 1.4;
		}

		.hero .text h3 {
			font-size: 24px;
			font-weight: 300;
		}

		.hero .text h2 span {
			font-weight: 600;
			color: #000;
		}

		.text-author {
			bordeR: 1px solid rgba(0, 0, 0, .05);
			max-width: 50%;
			margin: 0 auto;
			padding: 2em;
		}

		.text-author img {
			border-radius: 50%;
			padding-bottom: 20px;
		}

		.text-author h3 {
			margin-bottom: 0;
		}

		ul.social {
			padding: 0;
		}

		ul.social li {
			display: inline-block;
			margin-right: 10px;
		}

		/*FOOTER*/
		.footer {
			border-top: 1px solid rgba(0, 0, 0, .05);
			color: rgba(0, 0, 0, .5);
		}

		.footer .heading {
			color: #000;
			font-size: 20px;
		}

		.footer ul {
			margin: 0;
			padding: 0;
		}

		.footer ul li {
			list-style: none;
			margin-bottom: 10px;
		}

		.footer ul li a {
			color: rgba(0, 0, 0, 1);
		}

	</style>
</head>

<body width='100%' style='margin: 0; padding: 0 !important; mso-line-height-rule: exactly; background-color: #f1f1f1;'>
	<center style='width: 100%;'>
		<div
			style='display: none; font-size: 1px;max-height: 0px; max-width: 0px; opacity: 0; overflow: hidden; mso-hide: all; font-family: sans-serif;'>
			&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;
		</div>
		<div style='max-width: 600px; margin: 0 auto;' class='email-container'>
			<!-- BEGIN BODY -->
			<table align='center' role='presentation' cellspacing='0' cellpadding='0' border='0' width='100%'
				style='margin: auto;'>
				<tr>
					<td valign='top' class='bg_white' style='padding: 1em 2.5em 0 2.5em;'>
						<table role='presentation' border='0' cellpadding='0' cellspacing='0' width='100%'>
							<tr>
								<td class='logo' style='text-align: center;'>
									<h1><a href='#'><img src='" + appdata.emailLogoUrl + @"'
												style='width: 250px; height: auto;'></a></h1>
								</td>
							</tr>
						</table>
					</td>
				</tr><!-- end tr -->
				<tr>
					<td valign='middle' class='hero bg_white' style='padding: 2em 0 4em 0;'>
						<table role='presentation' border='0' cellpadding='0' cellspacing='0' width='100%'>
							<tr>
								<td style='padding: 0 2.5em; text-align: center; padding-bottom: 3em;'>
									<div class='text'>
										<h2>Witaj #name&nbsp;#lastname<br /></h2><br>
									</div>
								</td>
							</tr>
							<tr>
								<td style='text-align: center;'>
									<div class='text-author'>
										<h3 class='name'>Twój samochód jest gotowy do odbioru!</h3> <br />
										<p><a href='" + appdata.loginUrl + @"' class='btn btn-primary'>Przejdź do logowania</a></p>
									</div>
								</td>
							</tr>
						</table>
					</td>
				</tr><!-- end tr -->
				<!-- 1 Column Text + Button : END -->
			</table>
			<table align='center' role='presentation' cellspacing='0' cellpadding='0' border='0' width='100%'
				style='margin: auto;'>
				<tr>
					<td valign='middle' class='bg_light footer email-section'>
						<table>
							<tr>
								<td valign='top' width='50%' style='padding-top: 20px;'>
									<table role='presentation' cellspacing='0' cellpadding='0' border='0' width='100%'>
										<tr>
											<td style='text-align: left; padding-left: 5px; padding-right: 5px;'>
												<h3 class='heading'>Kontakt:</h3>
												<ul>
													<li><span class='text'>" + appdata.companyName + @".<br />" + appdata.companyAddress + @"<br />" + appdata.companyPostcode + @" " + appdata.companyCity + @"</span></li>
													<li><span class='text'>+48 " + appdata.companyPhone + @"</span></li>
                                                    <li><span class='text'>" + appdata.companyEmail + @"</span></li>
												</ul>
											</td>
										</tr>
									</table>
								</td>
							</tr>
						</table>
					</td>
				</tr><!-- end: tr -->
				<tr> </tr>
			</table>
		</div>
	</center>
</body>

</html>";

        public static string SendAddClientMessageTitle = $"{appdata.companyName} - Twoje nowe konto w naszym warsztacie!";

        public static string SendAddClientMessage = @"<!DOCTYPE html>
<html lang='pl' xmlns='http://www.w3.org/1999/xhtml' xmlns:v='urn:schemas-microsoft-com:vml'
	xmlns:o='urn:schemas-microsoft-com:office:office'>

<head>
	<meta charset='utf-8'> <!-- utf-8 works for most cases -->
	<meta name='viewport' content='width=device-width'> <!-- Forcing initial-scale shouldn't be necessary -->
	<meta http-equiv='X-UA-Compatible' content='IE=edge'> <!-- Use the latest (edge) version of IE rendering engine -->
	<meta name='x-apple-disable-message-reformatting'> <!-- Disable auto-scale in iOS 10 Mail entirely -->
	<title>#title</title> <!-- The title tag shows in email notifications, like Android 4.4. -->
	<link href='https://fonts.googleapis.com/css?family=Poppins:200,300,400,500,600,700' rel='stylesheet'>
	<!-- CSS Reset : BEGIN -->
	<style>
		/* What it does: Remove spaces around the email design added by some email clients. */
		/* Beware: It can remove the padding / margin and add a background color to the compose a reply window. */
		html,
		body {
			margin: 0 auto !important;
			padding: 0 !important;
			height: 100% !important;
			width: 100% !important;
			background: #f1f1f1;
		}

		/* What it does: Stops email clients resizing small text. */
		* {
			-ms-text-size-adjust: 100%;
			-webkit-text-size-adjust: 100%;
		}

		/* What it does: Centers email on Android 4.4 */
		div[style*='margin: 16px 0'] {
			margin: 0 !important;
		}

		/* What it does: Stops Outlook from adding extra spacing to tables. */
		table,
		td {
			mso-table-lspace: 0pt !important;
			mso-table-rspace: 0pt !important;
		}

		/* What it does: Fixes webkit padding issue. */
		table {
			border-spacing: 0 !important;
			border-collapse: collapse !important;
			table-layout: fixed !important;
			margin: 0 auto !important;
		}

		/* What it does: Uses a better rendering method when resizing images in IE. */
		img {
			-ms-interpolation-mode: bicubic;
		}

		/* What it does: Prevents Windows 10 Mail from underlining links despite inline CSS. Styles for underlined links should be inline. */
		a {
			text-decoration: none;
		}

		/* What it does: A work-around for email clients meddling in triggered links. */
		*[x-apple-data-detectors],
		/* iOS */
		.unstyle-auto-detected-links *,
		.aBn {
			border-bottom: 0 !important;
			cursor: default !important;
			color: inherit !important;
			text-decoration: none !important;
			font-size: inherit !important;
			font-family: inherit !important;
			font-weight: inherit !important;
			line-height: inherit !important;
		}

		/* What it does: Prevents Gmail from displaying a download button on large, non-linked images. */
		.a6S {
			display: none !important;
			opacity: 0.01 !important;
		}

		/* What it does: Prevents Gmail from changing the text color in conversation threads. */
		.im {
			color: inherit !important;
		}

		/* If the above doesn't work, add a .g-img class to any image in question. */
		img.g-img+div {
			display: none !important;
		}

		/* What it does: Removes right gutter in Gmail iOS app: https://github.com/TedGoas/Cerberus/issues/89  */
		/* Create one of these media queries for each additional viewport size you'd like to fix */
		/* iPhone 4, 4S, 5, 5S, 5C, and 5SE */
		@media only screen and (min-device-width: 320px) and (max-device-width: 374px) {
			u~div .email-container {
				min-width: 320px !important;
			}
		}

		/* iPhone 6, 6S, 7, 8, and X */
		@media only screen and (min-device-width: 375px) and (max-device-width: 413px) {
			u~div .email-container {
				min-width: 375px !important;
			}
		}

		/* iPhone 6+, 7+, and 8+ */
		@media only screen and (min-device-width: 414px) {
			u~div .email-container {
				min-width: 414px !important;
			}
		}
	</style> <!-- CSS Reset : END -->
	<style>
		/* What it does: Remove spaces around the email design added by some email clients. */
		/* Beware: It can remove the padding / margin and add a background color to the compose a reply window. */
		html,
		body {
			margin: 0 auto !important;
			padding: 0 !important;
			height: 100% !important;
			width: 100% !important;
			background: #f1f1f1;
		}

		/* What it does: Stops email clients resizing small text. */
		* {
			-ms-text-size-adjust: 100%;
			-webkit-text-size-adjust: 100%;
		}

		/* What it does: Centers email on Android 4.4 */
		div[style*='margin: 16px 0'] {
			margin: 0 !important;
		}

		/* What it does: Stops Outlook from adding extra spacing to tables. */
		table,
		td {
			mso-table-lspace: 0pt !important;
			mso-table-rspace: 0pt !important;
		}

		/* What it does: Fixes webkit padding issue. */
		table {
			border-spacing: 0 !important;
			border-collapse: collapse !important;
			table-layout: fixed !important;
			margin: 0 auto !important;
		}

		/* What it does: Uses a better rendering method when resizing images in IE. */
		img {
			-ms-interpolation-mode: bicubic;
		}

		/* What it does: Prevents Windows 10 Mail from underlining links despite inline CSS. Styles for underlined links should be inline. */
		a {
			text-decoration: none;
		}

		/* What it does: A work-around for email clients meddling in triggered links. */
		*[x-apple-data-detectors],
		/* iOS */
		.unstyle-auto-detected-links *,
		.aBn {
			border-bottom: 0 !important;
			cursor: default !important;
			color: inherit !important;
			text-decoration: none !important;
			font-size: inherit !important;
			font-family: inherit !important;
			font-weight: inherit !important;
			line-height: inherit !important;
		}

		/* What it does: Prevents Gmail from displaying a download button on large, non-linked images. */
		.a6S {
			display: none !important;
			opacity: 0.01 !important;
		}

		/* What it does: Prevents Gmail from changing the text color in conversation threads. */
		.im {
			color: inherit !important;
		}

		/* If the above doesn't work, add a .g-img class to any image in question. */
		img.g-img+div {
			display: none !important;
		}

		/* What it does: Removes right gutter in Gmail iOS app: https://github.com/TedGoas/Cerberus/issues/89  */
		/* Create one of these media queries for each additional viewport size you'd like to fix */
		/* iPhone 4, 4S, 5, 5S, 5C, and 5SE */
		@media only screen and (min-device-width: 320px) and (max-device-width: 374px) {
			u~div .email-container {
				min-width: 320px !important;
			}
		}

		/* iPhone 6, 6S, 7, 8, and X */
		@media only screen and (min-device-width: 375px) and (max-device-width: 413px) {
			u~div .email-container {
				min-width: 375px !important;
			}
		}

		/* iPhone 6+, 7+, and 8+ */
		@media only screen and (min-device-width: 414px) {
			u~div .email-container {
				min-width: 414px !important;
			}
		}
	</style> <!-- CSS Reset : END -->
	<!-- Progressive Enhancements : BEGIN -->
	<style>
		.primary {
			background: #17bebb;
		}

		.bg_white {
			background: #ffffff;
		}

		.bg_light {
			background: #f7fafa;
		}

		.bg_black {
			background: #000000;
		}

		.bg_dark {
			background: rgba(0, 0, 0, .8);
		}

		.email-section {
			padding: 2.5em;
		}

		/*BUTTON*/
		.btn {
			padding: 10px 15px;
			display: inline-block;
		}

		.btn.btn-primary {
			border-radius: 5px;
			background: #17bebb;
			color: #ffffff;
		}

		.btn.btn-white {
			border-radius: 5px;
			background: #ffffff;
			color: #000000;
		}

		.btn.btn-white-outline {
			border-radius: 5px;
			background: transparent;
			border: 1px solid #fff;
			color: #fff;
		}

		.btn.btn-black-outline {
			border-radius: 0px;
			background: transparent;
			border: 2px solid #000;
			color: #000;
			font-weight: 700;
		}

		.btn-custom {
			color: rgba(0, 0, 0, .3);
			text-decoration: underline;
		}

		h1,
		h2,
		h3,
		h4,
		h5,
		h6 {
			font-family: 'Poppins', sans-serif;
			color: #000000;
			margin-top: 0;
			font-weight: 400;
		}

		body {
			font-family: 'Poppins', sans-serif;
			font-weight: 400;
			font-size: 15px;
			line-height: 1.8;
			color: rgba(0, 0, 0, .4);
		}

		a {
			color: #17bebb;
		}

		table {}

		/*LOGO*/
		.logo h1 {
			margin: 0;
		}

		.logo h1 a {
			color: #17bebb;
			font-size: 24px;
			font-weight: 700;
			font-family: 'Poppins', sans-serif;
		}

		/*HERO*/
		.hero {
			position: relative;
			z-index: 0;
		}

		.hero .text {
			color: rgba(0, 0, 0, .3);
		}

		.hero .text h2 {
			color: #000;
			font-size: 34px;
			margin-bottom: 0;
			font-weight: 200;
			line-height: 1.4;
		}

		.hero .text h3 {
			font-size: 24px;
			font-weight: 300;
		}

		.hero .text h2 span {
			font-weight: 600;
			color: #000;
		}

		.text-author {
			bordeR: 1px solid rgba(0, 0, 0, .05);
			max-width: 50%;
			margin: 0 auto;
			padding: 2em;
		}

		.text-author img {
			border-radius: 50%;
			padding-bottom: 20px;
		}

		.text-author h3 {
			margin-bottom: 0;
		}

		ul.social {
			padding: 0;
		}

		ul.social li {
			display: inline-block;
			margin-right: 10px;
		}

		/*FOOTER*/
		.footer {
			border-top: 1px solid rgba(0, 0, 0, .05);
			color: rgba(0, 0, 0, .5);
		}

		.footer .heading {
			color: #000;
			font-size: 20px;
		}

		.footer ul {
			margin: 0;
			padding: 0;
		}

		.footer ul li {
			list-style: none;
			margin-bottom: 10px;
		}

		.footer ul li a {
			color: rgba(0, 0, 0, 1);
		}

		@media screen and (max-width: 500px) {}
	</style> <!-- CSS Reset : END -->
	<!-- Progressive Enhancements : BEGIN -->
	
	<!-- Progressive Enhancements : BEGIN -->
</head>

<body width='100%' style='margin: 0; padding: 0 !important; mso-line-height-rule: exactly; background-color: #f1f1f1;'>
	<center style='width: 100%;'>
		<div
			style='display: none; font-size: 1px;max-height: 0px; max-width: 0px; opacity: 0; overflow: hidden; mso-hide: all; font-family: sans-serif;'>
			&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;
		</div>
		<div style='max-width: 600px; margin: 0 auto;' class='email-container'>
			<!-- BEGIN BODY -->
			<table align='center' role='presentation' cellspacing='0' cellpadding='0' border='0' width='100%'
				style='margin: auto;'>
				<tr>
					<td valign='top' class='bg_white' style='padding: 1em 2.5em 0 2.5em;'>
						<table role='presentation' border='0' cellpadding='0' cellspacing='0' width='100%'>
							<tr>
								<td class='logo' style='text-align: center;'>
									<h1><a href='#'><img src='" + appdata.emailLogoUrl + @"'
												style='width: 250px; height: auto;'></a></h1>
								</td>
							</tr>
						</table>
					</td>
				</tr><!-- end tr -->
				<tr>
					<td valign='middle' class='hero bg_white' style='padding: 2em 0 4em 0;'>
						<table role='presentation' border='0' cellpadding='0' cellspacing='0' width='100%'>
							<tr>
								<td style='padding: 0 2.5em; text-align: center; padding-bottom: 3em;'>
									<div class='text'>
										<h2>Witaj #name #lastname<br /></h2><br>
                                        <h5>Dziękujemy za dołączenie do naszej społeczności. Jesteśmy gotowi na obsługę Twojego pojazdu. Czekamy na Ciebie w naszym warsztacie!</h5>
									</div>
								</td>
							</tr>
							<tr>
								<td style='text-align: center;'>
									<div class='text-author'>
										<h3 class='name'>Twoje dane logowania</h3> <br />
										<h4 class='position'>Login: #login</h4>
										<h4 class='position'>Hasło: #password</h4>
										<p><a href='" + appdata.loginUrl + @"' class='btn btn-primary'>Przejdź do logowania</a></p>
									</div>
								</td>
							</tr>
						</table>
					</td>
				</tr><!-- end tr -->
				<!-- 1 Column Text + Button : END -->
			</table>
			<table align='center' role='presentation' cellspacing='0' cellpadding='0' border='0' width='100%'
				style='margin: auto;'>
				<tr>
					<td valign='middle' class='bg_light footer email-section'>
						<table>
							<tr>
								<td valign='top' width='50%' style='padding-top: 20px;'>
									<table role='presentation' cellspacing='0' cellpadding='0' border='0' width='100%'>
										<tr>
											<td style='text-align: left; padding-left: 5px; padding-right: 5px;'>
												<h3 class='heading'>Kontakt:</h3>
												<ul>
													<li><span class='text'>" + appdata.companyName + @".<br />" + appdata.companyAddress + @"<br />" + appdata.companyPostcode + @" " + appdata.companyCity + @"</span></li>
													<li><span class='text'>+48 " + appdata.companyPhone + @"</span></li>
                                                    <li><span class='text'>" + appdata.companyEmail + @"</span></li>
												</ul>
											</td>
										</tr>
									</table>
								</td>
							</tr>
						</table>
					</td>
				</tr><!-- end: tr -->
				<tr> </tr>
			</table>
		</div>
	</center>
</body>

</html>";

        public static string SendContactConfirmationTitle = "Potwierdzenie wysłania wiadomości!";

        public static string SendContactConfirmationMessage = @"<!DOCTYPE html>
<html lang='pl' xmlns='http://www.w3.org/1999/xhtml' xmlns:v='urn:schemas-microsoft-com:vml'
	xmlns:o='urn:schemas-microsoft-com:office:office'>

<head>
	<meta charset='utf-8'> <!-- utf-8 works for most cases -->
	<meta name='viewport' content='width=device-width'> <!-- Forcing initial-scale shouldn't be necessary -->
	<meta http-equiv='X-UA-Compatible' content='IE=edge'> <!-- Use the latest (edge) version of IE rendering engine -->
	<meta name='x-apple-disable-message-reformatting'> <!-- Disable auto-scale in iOS 10 Mail entirely -->
	<title>#title</title> <!-- The title tag shows in email notifications, like Android 4.4. -->
	<link href='https://fonts.googleapis.com/css?family=Poppins:200,300,400,500,600,700' rel='stylesheet'>
	<!-- CSS Reset : BEGIN -->
	<style>
		/* What it does: Remove spaces around the email design added by some email clients. */
		/* Beware: It can remove the padding / margin and add a background color to the compose a reply window. */
		html,
		body {
			margin: 0 auto !important;
			padding: 0 !important;
			height: 100% !important;
			width: 100% !important;
			background: #f1f1f1;
		}

		/* What it does: Stops email clients resizing small text. */
		* {
			-ms-text-size-adjust: 100%;
			-webkit-text-size-adjust: 100%;
		}

		/* What it does: Centers email on Android 4.4 */
		div[style*='margin: 16px 0'] {
			margin: 0 !important;
		}

		/* What it does: Stops Outlook from adding extra spacing to tables. */
		table,
		td {
			mso-table-lspace: 0pt !important;
			mso-table-rspace: 0pt !important;
		}

		/* What it does: Fixes webkit padding issue. */
		table {
			border-spacing: 0 !important;
			border-collapse: collapse !important;
			table-layout: fixed !important;
			margin: 0 auto !important;
		}

		/* What it does: Uses a better rendering method when resizing images in IE. */
		img {
			-ms-interpolation-mode: bicubic;
		}

		/* What it does: Prevents Windows 10 Mail from underlining links despite inline CSS. Styles for underlined links should be inline. */
		a {
			text-decoration: none;
		}

		/* What it does: A work-around for email clients meddling in triggered links. */
		*[x-apple-data-detectors],
		/* iOS */
		.unstyle-auto-detected-links *,
		.aBn {
			border-bottom: 0 !important;
			cursor: default !important;
			color: inherit !important;
			text-decoration: none !important;
			font-size: inherit !important;
			font-family: inherit !important;
			font-weight: inherit !important;
			line-height: inherit !important;
		}

		/* What it does: Prevents Gmail from displaying a download button on large, non-linked images. */
		.a6S {
			display: none !important;
			opacity: 0.01 !important;
		}

		/* What it does: Prevents Gmail from changing the text color in conversation threads. */
		.im {
			color: inherit !important;
		}

		/* If the above doesn't work, add a .g-img class to any image in question. */
		img.g-img+div {
			display: none !important;
		}

		/* What it does: Removes right gutter in Gmail iOS app: https://github.com/TedGoas/Cerberus/issues/89  */
		/* Create one of these media queries for each additional viewport size you'd like to fix */
		/* iPhone 4, 4S, 5, 5S, 5C, and 5SE */
		@media only screen and (min-device-width: 320px) and (max-device-width: 374px) {
			u~div .email-container {
				min-width: 320px !important;
			}
		}

		/* iPhone 6, 6S, 7, 8, and X */
		@media only screen and (min-device-width: 375px) and (max-device-width: 413px) {
			u~div .email-container {
				min-width: 375px !important;
			}
		}

		/* iPhone 6+, 7+, and 8+ */
		@media only screen and (min-device-width: 414px) {
			u~div .email-container {
				min-width: 414px !important;
			}
		}
	</style> <!-- CSS Reset : END -->
	<style>
		/* What it does: Remove spaces around the email design added by some email clients. */
		/* Beware: It can remove the padding / margin and add a background color to the compose a reply window. */
		html,
		body {
			margin: 0 auto !important;
			padding: 0 !important;
			height: 100% !important;
			width: 100% !important;
			background: #f1f1f1;
		}

		/* What it does: Stops email clients resizing small text. */
		* {
			-ms-text-size-adjust: 100%;
			-webkit-text-size-adjust: 100%;
		}

		/* What it does: Centers email on Android 4.4 */
		div[style*='margin: 16px 0'] {
			margin: 0 !important;
		}

		/* What it does: Stops Outlook from adding extra spacing to tables. */
		table,
		td {
			mso-table-lspace: 0pt !important;
			mso-table-rspace: 0pt !important;
		}

		/* What it does: Fixes webkit padding issue. */
		table {
			border-spacing: 0 !important;
			border-collapse: collapse !important;
			table-layout: fixed !important;
			margin: 0 auto !important;
		}

		/* What it does: Uses a better rendering method when resizing images in IE. */
		img {
			-ms-interpolation-mode: bicubic;
		}

		/* What it does: Prevents Windows 10 Mail from underlining links despite inline CSS. Styles for underlined links should be inline. */
		a {
			text-decoration: none;
		}

		/* What it does: A work-around for email clients meddling in triggered links. */
		*[x-apple-data-detectors],
		/* iOS */
		.unstyle-auto-detected-links *,
		.aBn {
			border-bottom: 0 !important;
			cursor: default !important;
			color: inherit !important;
			text-decoration: none !important;
			font-size: inherit !important;
			font-family: inherit !important;
			font-weight: inherit !important;
			line-height: inherit !important;
		}

		/* What it does: Prevents Gmail from displaying a download button on large, non-linked images. */
		.a6S {
			display: none !important;
			opacity: 0.01 !important;
		}

		/* What it does: Prevents Gmail from changing the text color in conversation threads. */
		.im {
			color: inherit !important;
		}

		/* If the above doesn't work, add a .g-img class to any image in question. */
		img.g-img+div {
			display: none !important;
		}

		/* What it does: Removes right gutter in Gmail iOS app: https://github.com/TedGoas/Cerberus/issues/89  */
		/* Create one of these media queries for each additional viewport size you'd like to fix */
		/* iPhone 4, 4S, 5, 5S, 5C, and 5SE */
		@media only screen and (min-device-width: 320px) and (max-device-width: 374px) {
			u~div .email-container {
				min-width: 320px !important;
			}
		}

		/* iPhone 6, 6S, 7, 8, and X */
		@media only screen and (min-device-width: 375px) and (max-device-width: 413px) {
			u~div .email-container {
				min-width: 375px !important;
			}
		}

		/* iPhone 6+, 7+, and 8+ */
		@media only screen and (min-device-width: 414px) {
			u~div .email-container {
				min-width: 414px !important;
			}
		}
	</style> <!-- CSS Reset : END -->
	<!-- Progressive Enhancements : BEGIN -->
	<style>
		.primary {
			background: #17bebb;
		}

		.bg_white {
			background: #ffffff;
		}

		.bg_light {
			background: #f7fafa;
		}

		.bg_black {
			background: #000000;
		}

		.bg_dark {
			background: rgba(0, 0, 0, .8);
		}

		.email-section {
			padding: 2.5em;
		}

		/*BUTTON*/
		.btn {
			padding: 10px 15px;
			display: inline-block;
		}

		.btn.btn-primary {
			border-radius: 5px;
			background: #17bebb;
			color: #ffffff;
		}

		.btn.btn-white {
			border-radius: 5px;
			background: #ffffff;
			color: #000000;
		}

		.btn.btn-white-outline {
			border-radius: 5px;
			background: transparent;
			border: 1px solid #fff;
			color: #fff;
		}

		.btn.btn-black-outline {
			border-radius: 0px;
			background: transparent;
			border: 2px solid #000;
			color: #000;
			font-weight: 700;
		}

		.btn-custom {
			color: rgba(0, 0, 0, .3);
			text-decoration: underline;
		}

		h1,
		h2,
		h3,
		h4,
		h5,
		h6 {
			font-family: 'Poppins', sans-serif;
			color: #000000;
			margin-top: 0;
			font-weight: 400;
		}

		body {
			font-family: 'Poppins', sans-serif;
			font-weight: 400;
			font-size: 15px;
			line-height: 1.8;
			color: rgba(0, 0, 0, .4);
		}

		a {
			color: #17bebb;
		}

		table {}

		/*LOGO*/
		.logo h1 {
			margin: 0;
		}

		.logo h1 a {
			color: #17bebb;
			font-size: 24px;
			font-weight: 700;
			font-family: 'Poppins', sans-serif;
		}

		/*HERO*/
		.hero {
			position: relative;
			z-index: 0;
		}

		.hero .text {
			color: rgba(0, 0, 0, .3);
		}

		.hero .text h2 {
			color: #000;
			font-size: 34px;
			margin-bottom: 0;
			font-weight: 200;
			line-height: 1.4;
		}

		.hero .text h3 {
			font-size: 24px;
			font-weight: 300;
		}

		.hero .text h2 span {
			font-weight: 600;
			color: #000;
		}

		.text-author {
			bordeR: 1px solid rgba(0, 0, 0, .05);
			max-width: 50%;
			margin: 0 auto;
			padding: 2em;
		}

		.text-author img {
			border-radius: 50%;
			padding-bottom: 20px;
		}

		.text-author h3 {
			margin-bottom: 0;
		}

		ul.social {
			padding: 0;
		}

		ul.social li {
			display: inline-block;
			margin-right: 10px;
		}

		/*FOOTER*/
		.footer {
			border-top: 1px solid rgba(0, 0, 0, .05);
			color: rgba(0, 0, 0, .5);
		}

		.footer .heading {
			color: #000;
			font-size: 20px;
		}

		.footer ul {
			margin: 0;
			padding: 0;
		}

		.footer ul li {
			list-style: none;
			margin-bottom: 10px;
		}

		.footer ul li a {
			color: rgba(0, 0, 0, 1);
		}

		@media screen and (max-width: 500px) {}
	</style> <!-- CSS Reset : END -->
	<!-- Progressive Enhancements : BEGIN -->
	
	<!-- Progressive Enhancements : BEGIN -->
</head>

<body width='100%' style='margin: 0; padding: 0 !important; mso-line-height-rule: exactly; background-color: #f1f1f1;'>
	<center style='width: 100%;'>
		<div
			style='display: none; font-size: 1px;max-height: 0px; max-width: 0px; opacity: 0; overflow: hidden; mso-hide: all; font-family: sans-serif;'>
			&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;
		</div>
		<div style='max-width: 600px; margin: 0 auto;' class='email-container'>
			<!-- BEGIN BODY -->
			<table align='center' role='presentation' cellspacing='0' cellpadding='0' border='0' width='100%'
				style='margin: auto;'>
				<tr>
					<td valign='top' class='bg_white' style='padding: 1em 2.5em 0 2.5em;'>
						<table role='presentation' border='0' cellpadding='0' cellspacing='0' width='100%'>
							<tr>
								<td class='logo' style='text-align: center;'>
									<h1><a href='#'><img src='" + appdata.emailLogoUrl + @"'
												style='width: 250px; height: auto;'></a></h1>
								</td>
							</tr>
						</table>
					</td>
				</tr><!-- end tr -->
				<tr>
					<td valign='middle' class='hero bg_white' style='padding: 2em 0 4em 0;'>
						<table role='presentation' border='0' cellpadding='0' cellspacing='0' width='100%'>
							<tr>
								<td style='padding: 0 2.5em; text-align: center; padding-bottom: 3em;'>
									<div class='text'>
										<h2>Witaj #name #lastname<br /></h2><br>
                                        <h5>Twoja wiadomość została wysłana: </h5><br>
										<h5>#mess</h5>
									</div>
								</td>
							</tr>
						</table>
					</td>
				</tr><!-- end tr -->
				<!-- 1 Column Text + Button : END -->
			</table>
			<table align='center' role='presentation' cellspacing='0' cellpadding='0' border='0' width='100%'
				style='margin: auto;'>
				<tr>
					<td valign='middle' class='bg_light footer email-section'>
						<table>
							<tr>
								<td valign='top' width='50%' style='padding-top: 20px;'>
									<table role='presentation' cellspacing='0' cellpadding='0' border='0' width='100%'>
										<tr>
											<td style='text-align: left; padding-left: 5px; padding-right: 5px;'>
												<h3 class='heading'>Kontakt:</h3>
												<ul>
													<li><span class='text'>" + appdata.companyName + @".<br />" + appdata.companyAddress + @"<br />" + appdata.companyPostcode + @" " + appdata.companyCity + @"</span></li>
													<li><span class='text'>+48 " + appdata.companyPhone + @"</span></li>
                                                    <li><span class='text'>" + appdata.companyEmail + @"</span></li>
												</ul>
											</td>
										</tr>
									</table>
								</td>
							</tr>
						</table>
					</td>
				</tr><!-- end: tr -->
				<tr> </tr>
			</table>
		</div>
	</center>
</body>

</html>";

        public static string SendComplaintStartTitle = $"{appdata.companyName} - Twoja reklamacja jest rozpatrywana!";

        public static string SendComplaintStartMessage = @"<!DOCTYPE html>
<html lang='pl' xmlns='http://www.w3.org/1999/xhtml' xmlns:v='urn:schemas-microsoft-com:vml'
	xmlns:o='urn:schemas-microsoft-com:office:office'>

<head>
	<meta charset='utf-8'> <!-- utf-8 works for most cases -->
	<meta name='viewport' content='width=device-width'> <!-- Forcing initial-scale shouldn't be necessary -->
	<meta http-equiv='X-UA-Compatible' content='IE=edge'> <!-- Use the latest (edge) version of IE rendering engine -->
	<meta name='x-apple-disable-message-reformatting'> <!-- Disable auto-scale in iOS 10 Mail entirely -->
	<title>#title</title> <!-- The title tag shows in email notifications, like Android 4.4. -->
	<link href='https://fonts.googleapis.com/css?family=Poppins:200,300,400,500,600,700' rel='stylesheet'>
	<!-- CSS Reset : BEGIN -->
	<style>
		/* What it does: Remove spaces around the email design added by some email clients. */
		/* Beware: It can remove the padding / margin and add a background color to the compose a reply window. */
		html,
		body {
			margin: 0 auto !important;
			padding: 0 !important;
			height: 100% !important;
			width: 100% !important;
			background: #f1f1f1;
		}

		/* What it does: Stops email clients resizing small text. */
		* {
			-ms-text-size-adjust: 100%;
			-webkit-text-size-adjust: 100%;
		}

		/* What it does: Centers email on Android 4.4 */
		div[style*='margin: 16px 0'] {
			margin: 0 !important;
		}

		/* What it does: Stops Outlook from adding extra spacing to tables. */
		table,
		td {
			mso-table-lspace: 0pt !important;
			mso-table-rspace: 0pt !important;
		}

		/* What it does: Fixes webkit padding issue. */
		table {
			border-spacing: 0 !important;
			border-collapse: collapse !important;
			table-layout: fixed !important;
			margin: 0 auto !important;
		}

		/* What it does: Uses a better rendering method when resizing images in IE. */
		img {
			-ms-interpolation-mode: bicubic;
		}

		/* What it does: Prevents Windows 10 Mail from underlining links despite inline CSS. Styles for underlined links should be inline. */
		a {
			text-decoration: none;
		}

		/* What it does: A work-around for email clients meddling in triggered links. */
		*[x-apple-data-detectors],
		/* iOS */
		.unstyle-auto-detected-links *,
		.aBn {
			border-bottom: 0 !important;
			cursor: default !important;
			color: inherit !important;
			text-decoration: none !important;
			font-size: inherit !important;
			font-family: inherit !important;
			font-weight: inherit !important;
			line-height: inherit !important;
		}

		/* What it does: Prevents Gmail from displaying a download button on large, non-linked images. */
		.a6S {
			display: none !important;
			opacity: 0.01 !important;
		}

		/* What it does: Prevents Gmail from changing the text color in conversation threads. */
		.im {
			color: inherit !important;
		}

		/* If the above doesn't work, add a .g-img class to any image in question. */
		img.g-img+div {
			display: none !important;
		}

		/* What it does: Removes right gutter in Gmail iOS app: https://github.com/TedGoas/Cerberus/issues/89  */
		/* Create one of these media queries for each additional viewport size you'd like to fix */
		/* iPhone 4, 4S, 5, 5S, 5C, and 5SE */
		@media only screen and (min-device-width: 320px) and (max-device-width: 374px) {
			u~div .email-container {
				min-width: 320px !important;
			}
		}

		/* iPhone 6, 6S, 7, 8, and X */
		@media only screen and (min-device-width: 375px) and (max-device-width: 413px) {
			u~div .email-container {
				min-width: 375px !important;
			}
		}

		/* iPhone 6+, 7+, and 8+ */
		@media only screen and (min-device-width: 414px) {
			u~div .email-container {
				min-width: 414px !important;
			}
		}
	</style> <!-- CSS Reset : END -->
	<style>
		/* What it does: Remove spaces around the email design added by some email clients. */
		/* Beware: It can remove the padding / margin and add a background color to the compose a reply window. */
		html,
		body {
			margin: 0 auto !important;
			padding: 0 !important;
			height: 100% !important;
			width: 100% !important;
			background: #f1f1f1;
		}

		/* What it does: Stops email clients resizing small text. */
		* {
			-ms-text-size-adjust: 100%;
			-webkit-text-size-adjust: 100%;
		}

		/* What it does: Centers email on Android 4.4 */
		div[style*='margin: 16px 0'] {
			margin: 0 !important;
		}

		/* What it does: Stops Outlook from adding extra spacing to tables. */
		table,
		td {
			mso-table-lspace: 0pt !important;
			mso-table-rspace: 0pt !important;
		}

		/* What it does: Fixes webkit padding issue. */
		table {
			border-spacing: 0 !important;
			border-collapse: collapse !important;
			table-layout: fixed !important;
			margin: 0 auto !important;
		}

		/* What it does: Uses a better rendering method when resizing images in IE. */
		img {
			-ms-interpolation-mode: bicubic;
		}

		/* What it does: Prevents Windows 10 Mail from underlining links despite inline CSS. Styles for underlined links should be inline. */
		a {
			text-decoration: none;
		}

		/* What it does: A work-around for email clients meddling in triggered links. */
		*[x-apple-data-detectors],
		/* iOS */
		.unstyle-auto-detected-links *,
		.aBn {
			border-bottom: 0 !important;
			cursor: default !important;
			color: inherit !important;
			text-decoration: none !important;
			font-size: inherit !important;
			font-family: inherit !important;
			font-weight: inherit !important;
			line-height: inherit !important;
		}

		/* What it does: Prevents Gmail from displaying a download button on large, non-linked images. */
		.a6S {
			display: none !important;
			opacity: 0.01 !important;
		}

		/* What it does: Prevents Gmail from changing the text color in conversation threads. */
		.im {
			color: inherit !important;
		}

		/* If the above doesn't work, add a .g-img class to any image in question. */
		img.g-img+div {
			display: none !important;
		}

		/* What it does: Removes right gutter in Gmail iOS app: https://github.com/TedGoas/Cerberus/issues/89  */
		/* Create one of these media queries for each additional viewport size you'd like to fix */
		/* iPhone 4, 4S, 5, 5S, 5C, and 5SE */
		@media only screen and (min-device-width: 320px) and (max-device-width: 374px) {
			u~div .email-container {
				min-width: 320px !important;
			}
		}

		/* iPhone 6, 6S, 7, 8, and X */
		@media only screen and (min-device-width: 375px) and (max-device-width: 413px) {
			u~div .email-container {
				min-width: 375px !important;
			}
		}

		/* iPhone 6+, 7+, and 8+ */
		@media only screen and (min-device-width: 414px) {
			u~div .email-container {
				min-width: 414px !important;
			}
		}
	</style> <!-- CSS Reset : END -->
	<!-- Progressive Enhancements : BEGIN -->
	<style>
		.primary {
			background: #17bebb;
		}

		.bg_white {
			background: #ffffff;
		}

		.bg_light {
			background: #f7fafa;
		}

		.bg_black {
			background: #000000;
		}

		.bg_dark {
			background: rgba(0, 0, 0, .8);
		}

		.email-section {
			padding: 2.5em;
		}

		/*BUTTON*/
		.btn {
			padding: 10px 15px;
			display: inline-block;
		}

		.btn.btn-primary {
			border-radius: 5px;
			background: #17bebb;
			color: #ffffff;
		}

		.btn.btn-white {
			border-radius: 5px;
			background: #ffffff;
			color: #000000;
		}

		.btn.btn-white-outline {
			border-radius: 5px;
			background: transparent;
			border: 1px solid #fff;
			color: #fff;
		}

		.btn.btn-black-outline {
			border-radius: 0px;
			background: transparent;
			border: 2px solid #000;
			color: #000;
			font-weight: 700;
		}

		.btn-custom {
			color: rgba(0, 0, 0, .3);
			text-decoration: underline;
		}

		h1,
		h2,
		h3,
		h4,
		h5,
		h6 {
			font-family: 'Poppins', sans-serif;
			color: #000000;
			margin-top: 0;
			font-weight: 400;
		}

		body {
			font-family: 'Poppins', sans-serif;
			font-weight: 400;
			font-size: 15px;
			line-height: 1.8;
			color: rgba(0, 0, 0, .4);
		}

		a {
			color: #17bebb;
		}

		table {}

		/*LOGO*/
		.logo h1 {
			margin: 0;
		}

		.logo h1 a {
			color: #17bebb;
			font-size: 24px;
			font-weight: 700;
			font-family: 'Poppins', sans-serif;
		}

		/*HERO*/
		.hero {
			position: relative;
			z-index: 0;
		}

		.hero .text {
			color: rgba(0, 0, 0, .3);
		}

		.hero .text h2 {
			color: #000;
			font-size: 34px;
			margin-bottom: 0;
			font-weight: 200;
			line-height: 1.4;
		}

		.hero .text h3 {
			font-size: 24px;
			font-weight: 300;
		}

		.hero .text h2 span {
			font-weight: 600;
			color: #000;
		}

		.text-author {
			bordeR: 1px solid rgba(0, 0, 0, .05);
			max-width: 50%;
			margin: 0 auto;
			padding: 2em;
		}

		.text-author img {
			border-radius: 50%;
			padding-bottom: 20px;
		}

		.text-author h3 {
			margin-bottom: 0;
		}

		ul.social {
			padding: 0;
		}

		ul.social li {
			display: inline-block;
			margin-right: 10px;
		}

		/*FOOTER*/
		.footer {
			border-top: 1px solid rgba(0, 0, 0, .05);
			color: rgba(0, 0, 0, .5);
		}

		.footer .heading {
			color: #000;
			font-size: 20px;
		}

		.footer ul {
			margin: 0;
			padding: 0;
		}

		.footer ul li {
			list-style: none;
			margin-bottom: 10px;
		}

		.footer ul li a {
			color: rgba(0, 0, 0, 1);
		}

		@media screen and (max-width: 500px) {}
	</style> <!-- CSS Reset : END -->
	<!-- Progressive Enhancements : BEGIN -->
	
	<!-- Progressive Enhancements : BEGIN -->
</head>

<body width='100%' style='margin: 0; padding: 0 !important; mso-line-height-rule: exactly; background-color: #f1f1f1;'>
	<center style='width: 100%;'>
		<div
			style='display: none; font-size: 1px;max-height: 0px; max-width: 0px; opacity: 0; overflow: hidden; mso-hide: all; font-family: sans-serif;'>
			&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;
		</div>
		<div style='max-width: 600px; margin: 0 auto;' class='email-container'>
			<!-- BEGIN BODY -->
			<table align='center' role='presentation' cellspacing='0' cellpadding='0' border='0' width='100%'
				style='margin: auto;'>
				<tr>
					<td valign='top' class='bg_white' style='padding: 1em 2.5em 0 2.5em;'>
						<table role='presentation' border='0' cellpadding='0' cellspacing='0' width='100%'>
							<tr>
								<td class='logo' style='text-align: center;'>
									<h1><a href='#'><img src='" + appdata.emailLogoUrl + @"'
												style='width: 250px; height: auto;'></a></h1>
								</td>
							</tr>
						</table>
					</td>
				</tr><!-- end tr -->
				<tr>
					<td valign='middle' class='hero bg_white' style='padding: 2em 0 4em 0;'>
						<table role='presentation' border='0' cellpadding='0' cellspacing='0' width='100%'>
							<tr>
								<td style='padding: 0 2.5em; text-align: center; padding-bottom: 3em;'>
									<div class='text'>
										<h2>Witaj #name&nbsp;#lastname<br /></h2><br>
                                        <h5>Twoje zgłoszenie reklamacji do zlecenia #orderID jest rozpatrywane!<br />Poinformujemy Ciebie o decyzji.</h5>
									</div>
								</td>
							</tr>
							<tr>
								<td style='text-align: center;'>
									<div class='text-author'>
										<h3 class='name'>Jeżeli masz pytania, prosimy o kontakt telefoniczny lub przez formularz kontaktowy</h3> <br />
										<p><a href='" + appdata.loginUrl + @"/Contact' class='btn btn-primary'>Kontakt</a></p>
									</div>
								</td>
							</tr>
						</table>
					</td>
				</tr><!-- end tr -->
				<!-- 1 Column Text + Button : END -->
			</table>
			<table align='center' role='presentation' cellspacing='0' cellpadding='0' border='0' width='100%'
				style='margin: auto;'>
				<tr>
					<td valign='middle' class='bg_light footer email-section'>
						<table>
							<tr>
								<td valign='top' width='50%' style='padding-top: 20px;'>
									<table role='presentation' cellspacing='0' cellpadding='0' border='0' width='100%'>
										<tr>
											<td style='text-align: left; padding-left: 5px; padding-right: 5px;'>
												<h3 class='heading'>Kontakt:</h3>
												<ul>
													<li><span class='text'>" + appdata.companyName + @".<br />" + appdata.companyAddress + @"<br />" + appdata.companyPostcode + @" " + appdata.companyCity + @"</span></li>
													<li><span class='text'>+48 " + appdata.companyPhone + @"</span></li>
                                                    <li><span class='text'>" + appdata.companyEmail + @"</span></li>
												</ul>
											</td>
										</tr>
									</table>
								</td>
							</tr>
						</table>
					</td>
				</tr><!-- end: tr -->
				<tr> </tr>
			</table>
		</div>
	</center>
</body>

</html>";

        public static string SendComplaintDecisionTitle = $"{appdata.companyName} - Twoja reklamacja została #decision!";

        public static string SendComplaintDecisionMessage = @"<!DOCTYPE html>
<html lang='pl' xmlns='http://www.w3.org/1999/xhtml' xmlns:v='urn:schemas-microsoft-com:vml'
	xmlns:o='urn:schemas-microsoft-com:office:office'>

<head>
	<meta charset='utf-8'> <!-- utf-8 works for most cases -->
	<meta name='viewport' content='width=device-width'> <!-- Forcing initial-scale shouldn't be necessary -->
	<meta http-equiv='X-UA-Compatible' content='IE=edge'> <!-- Use the latest (edge) version of IE rendering engine -->
	<meta name='x-apple-disable-message-reformatting'> <!-- Disable auto-scale in iOS 10 Mail entirely -->
	<title>#title</title> <!-- The title tag shows in email notifications, like Android 4.4. -->
	<link href='https://fonts.googleapis.com/css?family=Poppins:200,300,400,500,600,700' rel='stylesheet'>
	<!-- CSS Reset : BEGIN -->
	<style>
		/* What it does: Remove spaces around the email design added by some email clients. */
		/* Beware: It can remove the padding / margin and add a background color to the compose a reply window. */
		html,
		body {
			margin: 0 auto !important;
			padding: 0 !important;
			height: 100% !important;
			width: 100% !important;
			background: #f1f1f1;
		}

		/* What it does: Stops email clients resizing small text. */
		* {
			-ms-text-size-adjust: 100%;
			-webkit-text-size-adjust: 100%;
		}

		/* What it does: Centers email on Android 4.4 */
		div[style*='margin: 16px 0'] {
			margin: 0 !important;
		}

		/* What it does: Stops Outlook from adding extra spacing to tables. */
		table,
		td {
			mso-table-lspace: 0pt !important;
			mso-table-rspace: 0pt !important;
		}

		/* What it does: Fixes webkit padding issue. */
		table {
			border-spacing: 0 !important;
			border-collapse: collapse !important;
			table-layout: fixed !important;
			margin: 0 auto !important;
		}

		/* What it does: Uses a better rendering method when resizing images in IE. */
		img {
			-ms-interpolation-mode: bicubic;
		}

		/* What it does: Prevents Windows 10 Mail from underlining links despite inline CSS. Styles for underlined links should be inline. */
		a {
			text-decoration: none;
		}

		/* What it does: A work-around for email clients meddling in triggered links. */
		*[x-apple-data-detectors],
		/* iOS */
		.unstyle-auto-detected-links *,
		.aBn {
			border-bottom: 0 !important;
			cursor: default !important;
			color: inherit !important;
			text-decoration: none !important;
			font-size: inherit !important;
			font-family: inherit !important;
			font-weight: inherit !important;
			line-height: inherit !important;
		}

		/* What it does: Prevents Gmail from displaying a download button on large, non-linked images. */
		.a6S {
			display: none !important;
			opacity: 0.01 !important;
		}

		/* What it does: Prevents Gmail from changing the text color in conversation threads. */
		.im {
			color: inherit !important;
		}

		/* If the above doesn't work, add a .g-img class to any image in question. */
		img.g-img+div {
			display: none !important;
		}

		/* What it does: Removes right gutter in Gmail iOS app: https://github.com/TedGoas/Cerberus/issues/89  */
		/* Create one of these media queries for each additional viewport size you'd like to fix */
		/* iPhone 4, 4S, 5, 5S, 5C, and 5SE */
		@media only screen and (min-device-width: 320px) and (max-device-width: 374px) {
			u~div .email-container {
				min-width: 320px !important;
			}
		}

		/* iPhone 6, 6S, 7, 8, and X */
		@media only screen and (min-device-width: 375px) and (max-device-width: 413px) {
			u~div .email-container {
				min-width: 375px !important;
			}
		}

		/* iPhone 6+, 7+, and 8+ */
		@media only screen and (min-device-width: 414px) {
			u~div .email-container {
				min-width: 414px !important;
			}
		}
	</style> <!-- CSS Reset : END -->
	<style>
		/* What it does: Remove spaces around the email design added by some email clients. */
		/* Beware: It can remove the padding / margin and add a background color to the compose a reply window. */
		html,
		body {
			margin: 0 auto !important;
			padding: 0 !important;
			height: 100% !important;
			width: 100% !important;
			background: #f1f1f1;
		}

		/* What it does: Stops email clients resizing small text. */
		* {
			-ms-text-size-adjust: 100%;
			-webkit-text-size-adjust: 100%;
		}

		/* What it does: Centers email on Android 4.4 */
		div[style*='margin: 16px 0'] {
			margin: 0 !important;
		}

		/* What it does: Stops Outlook from adding extra spacing to tables. */
		table,
		td {
			mso-table-lspace: 0pt !important;
			mso-table-rspace: 0pt !important;
		}

		/* What it does: Fixes webkit padding issue. */
		table {
			border-spacing: 0 !important;
			border-collapse: collapse !important;
			table-layout: fixed !important;
			margin: 0 auto !important;
		}

		/* What it does: Uses a better rendering method when resizing images in IE. */
		img {
			-ms-interpolation-mode: bicubic;
		}

		/* What it does: Prevents Windows 10 Mail from underlining links despite inline CSS. Styles for underlined links should be inline. */
		a {
			text-decoration: none;
		}

		/* What it does: A work-around for email clients meddling in triggered links. */
		*[x-apple-data-detectors],
		/* iOS */
		.unstyle-auto-detected-links *,
		.aBn {
			border-bottom: 0 !important;
			cursor: default !important;
			color: inherit !important;
			text-decoration: none !important;
			font-size: inherit !important;
			font-family: inherit !important;
			font-weight: inherit !important;
			line-height: inherit !important;
		}

		/* What it does: Prevents Gmail from displaying a download button on large, non-linked images. */
		.a6S {
			display: none !important;
			opacity: 0.01 !important;
		}

		/* What it does: Prevents Gmail from changing the text color in conversation threads. */
		.im {
			color: inherit !important;
		}

		/* If the above doesn't work, add a .g-img class to any image in question. */
		img.g-img+div {
			display: none !important;
		}

		/* What it does: Removes right gutter in Gmail iOS app: https://github.com/TedGoas/Cerberus/issues/89  */
		/* Create one of these media queries for each additional viewport size you'd like to fix */
		/* iPhone 4, 4S, 5, 5S, 5C, and 5SE */
		@media only screen and (min-device-width: 320px) and (max-device-width: 374px) {
			u~div .email-container {
				min-width: 320px !important;
			}
		}

		/* iPhone 6, 6S, 7, 8, and X */
		@media only screen and (min-device-width: 375px) and (max-device-width: 413px) {
			u~div .email-container {
				min-width: 375px !important;
			}
		}

		/* iPhone 6+, 7+, and 8+ */
		@media only screen and (min-device-width: 414px) {
			u~div .email-container {
				min-width: 414px !important;
			}
		}
	</style> <!-- CSS Reset : END -->
	<!-- Progressive Enhancements : BEGIN -->
	<style>
		.primary {
			background: #17bebb;
		}

		.bg_white {
			background: #ffffff;
		}

		.bg_light {
			background: #f7fafa;
		}

		.bg_black {
			background: #000000;
		}

		.bg_dark {
			background: rgba(0, 0, 0, .8);
		}

		.email-section {
			padding: 2.5em;
		}

		/*BUTTON*/
		.btn {
			padding: 10px 15px;
			display: inline-block;
		}

		.btn.btn-primary {
			border-radius: 5px;
			background: #17bebb;
			color: #ffffff;
		}

		.btn.btn-white {
			border-radius: 5px;
			background: #ffffff;
			color: #000000;
		}

		.btn.btn-white-outline {
			border-radius: 5px;
			background: transparent;
			border: 1px solid #fff;
			color: #fff;
		}

		.btn.btn-black-outline {
			border-radius: 0px;
			background: transparent;
			border: 2px solid #000;
			color: #000;
			font-weight: 700;
		}

		.btn-custom {
			color: rgba(0, 0, 0, .3);
			text-decoration: underline;
		}

		h1,
		h2,
		h3,
		h4,
		h5,
		h6 {
			font-family: 'Poppins', sans-serif;
			color: #000000;
			margin-top: 0;
			font-weight: 400;
		}

		body {
			font-family: 'Poppins', sans-serif;
			font-weight: 400;
			font-size: 15px;
			line-height: 1.8;
			color: rgba(0, 0, 0, .4);
		}

		a {
			color: #17bebb;
		}

		table {}

		/*LOGO*/
		.logo h1 {
			margin: 0;
		}

		.logo h1 a {
			color: #17bebb;
			font-size: 24px;
			font-weight: 700;
			font-family: 'Poppins', sans-serif;
		}

		/*HERO*/
		.hero {
			position: relative;
			z-index: 0;
		}

		.hero .text {
			color: rgba(0, 0, 0, .3);
		}

		.hero .text h2 {
			color: #000;
			font-size: 34px;
			margin-bottom: 0;
			font-weight: 200;
			line-height: 1.4;
		}

		.hero .text h3 {
			font-size: 24px;
			font-weight: 300;
		}

		.hero .text h2 span {
			font-weight: 600;
			color: #000;
		}

		.text-author {
			bordeR: 1px solid rgba(0, 0, 0, .05);
			max-width: 50%;
			margin: 0 auto;
			padding: 2em;
		}

		.text-author img {
			border-radius: 50%;
			padding-bottom: 20px;
		}

		.text-author h3 {
			margin-bottom: 0;
		}

		ul.social {
			padding: 0;
		}

		ul.social li {
			display: inline-block;
			margin-right: 10px;
		}

		/*FOOTER*/
		.footer {
			border-top: 1px solid rgba(0, 0, 0, .05);
			color: rgba(0, 0, 0, .5);
		}

		.footer .heading {
			color: #000;
			font-size: 20px;
		}

		.footer ul {
			margin: 0;
			padding: 0;
		}

		.footer ul li {
			list-style: none;
			margin-bottom: 10px;
		}

		.footer ul li a {
			color: rgba(0, 0, 0, 1);
		}

		@media screen and (max-width: 500px) {}
	</style> <!-- CSS Reset : END -->
	<!-- Progressive Enhancements : BEGIN -->
	
	<!-- Progressive Enhancements : BEGIN -->
</head>

<body width='100%' style='margin: 0; padding: 0 !important; mso-line-height-rule: exactly; background-color: #f1f1f1;'>
	<center style='width: 100%;'>
		<div
			style='display: none; font-size: 1px;max-height: 0px; max-width: 0px; opacity: 0; overflow: hidden; mso-hide: all; font-family: sans-serif;'>
			&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;
		</div>
		<div style='max-width: 600px; margin: 0 auto;' class='email-container'>
			<!-- BEGIN BODY -->
			<table align='center' role='presentation' cellspacing='0' cellpadding='0' border='0' width='100%'
				style='margin: auto;'>
				<tr>
					<td valign='top' class='bg_white' style='padding: 1em 2.5em 0 2.5em;'>
						<table role='presentation' border='0' cellpadding='0' cellspacing='0' width='100%'>
							<tr>
								<td class='logo' style='text-align: center;'>
									<h1><a href='#'><img src='" + appdata.emailLogoUrl + @"'
												style='width: 250px; height: auto;'></a></h1>
								</td>
							</tr>
						</table>
					</td>
				</tr><!-- end tr -->
				<tr>
					<td valign='middle' class='hero bg_white' style='padding: 2em 0 4em 0;'>
						<table role='presentation' border='0' cellpadding='0' cellspacing='0' width='100%'>
							<tr>
								<td style='padding: 0 2.5em; text-align: center; padding-bottom: 3em;'>
									<div class='text'>
										<h2>Witaj #name&nbsp;#lastname<br /></h2><br>
                                        <h5>Twoje zgłoszenie reklamacji do zlecenia #orderID zostało rozpatrzone #decision!<br /></h5>
										<h4>Powód:</h4><br/>
										<h5>#description</h5>
									</div>
								</td>
							</tr>
							<tr>
								<td style='text-align: center;'>
									<div class='text-author'>
										<h3 class='name'>Jeżeli masz pytania, prosimy o kontakt telefoniczny lub przez formularz kontaktowy</h3> <br />
										<p><a href='" + appdata.loginUrl + @"/Contact' class='btn btn-primary'>Kontakt</a></p>
									</div>
								</td>
							</tr>
						</table>
					</td>
				</tr><!-- end tr -->
				<!-- 1 Column Text + Button : END -->
			</table>
			<table align='center' role='presentation' cellspacing='0' cellpadding='0' border='0' width='100%'
				style='margin: auto;'>
				<tr>
					<td valign='middle' class='bg_light footer email-section'>
						<table>
							<tr>
								<td valign='top' width='50%' style='padding-top: 20px;'>
									<table role='presentation' cellspacing='0' cellpadding='0' border='0' width='100%'>
										<tr>
											<td style='text-align: left; padding-left: 5px; padding-right: 5px;'>
												<h3 class='heading'>Kontakt:</h3>
												<ul>
													<li><span class='text'>" + appdata.companyName + @".<br />" + appdata.companyAddress + @"<br />" + appdata.companyPostcode + @" " + appdata.companyCity + @"</span></li>
													<li><span class='text'>+48 " + appdata.companyPhone + @"</span></li>
                                                    <li><span class='text'>" + appdata.companyEmail + @"</span></li>
												</ul>
											</td>
										</tr>
									</table>
								</td>
							</tr>
						</table>
					</td>
				</tr><!-- end: tr -->
				<tr> </tr>
			</table>
		</div>
	</center>
</body>

</html>";
    }
}
