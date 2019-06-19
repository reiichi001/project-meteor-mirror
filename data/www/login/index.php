<?php 
		
	error_reporting(E_ALL | E_STRICT);

	include("config.php");
	include("database.php");	
		
	$loginError = "";
		
	function doLogin($dataConnection)
	{
		$username 		= trim($_POST["username"]);
		$password 		= trim($_POST["password"]);

		if(empty($username))
		{
			throw new Exception("You must enter an username.");
		}
		
		if(empty($password))
		{
			throw new Exception("You must enter a password.");
		}
	
		$userId = VerifyUser($dataConnection, $username, $password);
		return RefreshOrCreateSession($dataConnection, $userId);
	}

	$loginError = "";
	$currentTimeUTC = time();

	if(isset($_POST["login"]))
	{
		try
		{
			$sessionId = doLogin($g_databaseConnection);
		}
		catch(Exception $e)
		{
			$loginError = $e->getMessage();
		}
	}

?>

<html><head>
<meta http-equiv="content-type" content="text/html; charset=windows-1252">
		<style>
		html, body {
			font-family: Arial;
			font-size: 14px;
			margin:0;
			padding: 0;
			width: 600px;
		}

		html {
			padding: 10px;
		}

		form {
			background-color: #eee;
			border: solid 1px #666;
			border-radius: 3px;
			margin-bottom: 0;
		}

		table {
			justify-content: center;
			width: 100%;
			height: 100%;
			padding: 20px;
		}

		form input {
			width: 100%;
			border: solid 1px #222;
			padding: 3px;
			outline: none;
		}
				
		form button {
			background-image: url(img/btLogin.gif);
			background-position: 0 0;
			background-repeat: no-repeat;
			border: none;
			width: 200px;
			height: 40px;
		}
		form button:hover {
			background-position: 0 -40px;
			cursor: pointer;
		}
		
		.errorText{
			color: red;
		}
		
		.banner {
			margin-top: 10px;
		}
		</style>
	</head>
	
	<body>
			<?php if (isset($sessionId)) echo("<x-sqexauth sid=\"$sessionId\" lang=\"en-us\" region=\"2\" utc=\"$currentTimeUTC\" />"); ?>
			<table border="0" cellpadding="0" cellspacing="0">
			<tbody><tr>
				<td width="50%">
					<img src="img/logo.png" class="logo" width="300px">
				</td>
				<td width="50%">
					<form method="post">
						<table border="0" cellpadding="5px" cellspacing="0">
						<tbody><tr>
							<td width="5%"><img src="img/lbSQEXId_mem.gif"></td>
							<td width="40%"><label for="username">Username</label></td>
							<td width="50%"><input id="username" name="username" autocomplete="off" type="text"></td>
						</tr>
						<tr>
							<td><img src="img/lbSQEXPass_mem.gif" <="" td="">
							</td><td><label for="password">Password</label></td>
							<td><input id="password" name="password" autocomplete="off" type="password"></td>
						</tr>
						<tr>
							<td colspan="3" align="center">
							<p class=errorText><?php echo($loginError) ?></p>
							</td>
						</tr>
						<tr>
							<td colspan="3" align="center">
								<button type="submit" name="login">&nbsp;</button>
							</td>
						</tr>
						<tr>
							<td colspan="3" align="center">
								<a href="..\login_su\create_user.php">Don't have a awesome account?</a>
							</td>
						</tr>
						</tbody></table>
					
				</form></td>
			</tr>
			<tr>
				<td colspan="2" align="center">
					<img src="img/banner.png" class="banner" width="720px">
				</td>
			</tr>
			</tbody></table>

</body></html>