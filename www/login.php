<?php 

error_reporting(E_ALL | E_STRICT);

include("config.php");
include("database.php"); 

function LoginPage_Login($dataConnection)
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
$sessionId = "";

if(isset($_POST["login"]))
{
	try
	{
		$sessionId = LoginPage_Login($g_databaseConnection);
	}
	catch(Exception $e)
	{
		$loginError = $e->getMessage();
	}
}

?>
<!DOCTYPE HTML>
<html xmlns="http://www.w3.org/1999/xhtml">
	<head>
		<title>FFXIV 1.0 Login</title>
		<link rel="stylesheet" type="text/css" href="css/reset.css" />	
		<link rel="stylesheet" type="text/css" href="css/global.css" />	
	</head>
	<body>
		<?php
			if(!empty($sessionId))
			{
				echo "<script>window.location=\"ffxiv://login_success?sessionId=" . $sessionId . "\";</script>";
			}
		?>
			
			<div style="width: 80%; height: 300px; margin-left: auto; margin-right: auto; margin-top: 20%">
				<div class="contentContainer" >
					<img style="width: 100%;" src="./img/logo.png" />
				</div>
				
				<div class="contentContainer">
					<div class="info">
						<br />
						<form method="post" autocomplete="off">
							<table class="infoForm">
								<tr>
									<td>Username:</td>
									<td><input type="text" name="username" /></td>
								</tr>
								<tr>
									<td>Password:</td>
									<td><input type="password" name="password" /></td>
								</tr>
								<tr>
									<td colspan="2">
										<input type="submit" name="login" value="Login" />
									</td>
								</tr>
							</table>
						</form>
						<p class="errorMessage"><?php echo($loginError); ?></p>
					</div>
				</div>
			</div>
	
	</body>
</html>
