<?php
require 'patches.php';

function getCurrentUri()
{
	$basepath = implode('/', array_slice(explode('/', $_SERVER['SCRIPT_NAME']), 0, -1)) . '/';
	$uri = substr($_SERVER['REQUEST_URI'], strlen($basepath));
	if (strstr($uri, '?')) $uri = substr($uri, 0, strpos($uri, '?'));
	$uri = '/' . trim($uri, '/');
	return $uri;
}

function checkBootVersion($version)
{
	global $LATEST_BOOT_VERSION, $BOOT_PATCHES;
	if ($version != $LATEST_BOOT_VERSION)
	{
		if (!isset($BOOT_PATCHES[$version]))
		{
			header("HTTP/1.0 404 NOT FOUND");
			return;
		}
		
		header("HTTP/1.0 200 OK");
		header("Content-Location: ffxiv/2d2a390f/vercheck.dat");
		header("Content-Type: multipart/mixed; boundary=477D80B1_38BC_41d4_8B48_5273ADB89CAC");
		header("X-Repository: ffxiv/win32/release/boot");
		header("X-Patch-Module: ZiPatch");
		header("X-Protocol: torrent");
		header("X-Info-Url: http://example.com");
		header("X-Latest-Version: $LATEST_BOOT_VERSION");
		header("Connection: keep-alive");
		
		echo "--477D80B1_38BC_41d4_8B48_5273ADB89CAC\r\n";
		
		echo "Content-Type: application/octet-stream\r\n";
		echo "Content-Location: ffxiv/2d2a390f/metainfo/D$BOOT_PATCHES[$version].torrent\r\n";
		echo "X-Patch-Length: " . filesize("./ffxiv/2d2a390f/patch/D$BOOT_PATCHES[$version].patch") . "\r\n";
		echo "X-Signature: jqxmt9WQH1aXptNju6CmCdztFdaKbyOAVjdGw_DJvRiBJhnQL6UlDUcqxg2DeiIKhVzkjUm3hFXOVUFjygxCoPUmCwnbCaryNqVk_oTk_aZE4HGWNOEcAdBwf0Gb2SzwAtk69zs_5dLAtZ0mPpMuxWJiaNSvWjEmQ925BFwd7Vk=\r\n";
		
		echo "test: $version";
		
		echo "\r\n";
		readfile("./ffxiv/2d2a390f/metainfo/D$BOOT_PATCHES[$version].torrent");	
		echo "\r\n";
		
		echo "--477D80B1_38BC_41d4_8B48_5273ADB89CAC--\r\n\r\n";			
	}
	else
	{
		header("HTTP/1.0 204 No Content");
		header("Content-Location: ffxiv/48eca647/vercheck.dat");
		header("X-Repository: ffxiv/win32/release/boot");
		header("X-Patch-Module: ZiPatch");
		header("X-Protocol: torrent");
		header("X-Info-Url: http://www.example.com");
		header("X-Latest-Version: $LATEST_BOOT_VERSION");
	}
}

function checkGameVersion($version)
{
	global $LATEST_GAME_VERSION, $GAME_PATCHES;
	if ($version != $LATEST_GAME_VERSION)
	{
		if (!isset($GAME_PATCHES[$version]))
		{
			header("HTTP/1.0 404 NOT FOUND");
			return;
		}
		
		header("HTTP/1.0 200 OK");
		header("Content-Location: ffxiv/48eca647/vercheck.dat");
		header("Content-Type: multipart/mixed; boundary=477D80B1_38BC_41d4_8B48_5273ADB89CAC");
		header("X-Repository: ffxiv/win32/release/game");
		header("X-Patch-Module: ZiPatch");
		header("X-Protocol: torrent");
		header("X-Info-Url: http://example.com");
		header("X-Latest-Version: $LATEST_GAME_VERSION");
		header("Connection: keep-alive");
	
		echo "--477D80B1_38BC_41d4_8B48_5273ADB89CAC\r\n";
		
		echo "Content-Type: application/octet-stream\r\n";
		echo "Content-Location: ffxiv/48eca647/metainfo/D$GAME_PATCHES[$version].torrent\r\n";
		echo "X-Patch-Length: " . filesize("./ffxiv/48eca647/patch/D$GAME_PATCHES[$version].patch") . "\r\n";
		echo "X-Signature: jqxmt9WQH1aXptNju6CmCdztFdaKbyOAVjdGw_DJvRiBJhnQL6UlDUcqxg2DeiIKhVzkjUm3hFXOVUFjygxCoPUmCwnbCaryNqVk_oTk_aZE4HGWNOEcAdBwf0Gb2SzwAtk69zs_5dLAtZ0mPpMuxWJiaNSvWjEmQ925BFwd7Vk=\r\n";
		
		echo "\r\n";
		readfile("./ffxiv/48eca647/metainfo/D$GAME_PATCHES[$version].torrent");	
		echo "\r\n";
		
		echo "--477D80B1_38BC_41d4_8B48_5273ADB89CAC--\r\n\r\n";	
	}
	else
	{
		header("HTTP/1.0 204 No Content");
		header("Content-Location: ffxiv/48eca647/vercheck.dat");
		header("X-Repository: ffxiv/win32/release/game");
		header("X-Patch-Module: ZiPatch");
		header("X-Protocol: torrent");
		header("X-Info-Url: http://www.example.com");
		header("X-Latest-Version: $LATEST_GAME_VERSION");
	}
}

//Find the version request
$base_url = getCurrentUri();
$routes = array();
$routes = explode('/', $base_url);

//Are we even updating FFXIV?
if ($routes[1] == "ffxiv" && 
	$routes[2] == "win32" && 
	$routes[3] == "release"){
	//Updating Launcher
	if ($routes[4] == "boot")
		checkBootVersion($routes[5]);
	//Updating Game
	else if ($routes[4] == "game")
		checkGameVersion($routes[5]);
}	
?>