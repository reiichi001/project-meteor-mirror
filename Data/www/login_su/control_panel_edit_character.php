<?php 

error_reporting(E_ALL | E_STRICT);

include("config.php");
include("database.php"); 
include("control_panel_common.php");

if(!isset($_GET["id"]))
{
	header("Location: control_panel.php");
	exit;
}

$g_characterId = $_GET["id"];

$g_tribes = array(
	1 => "Hyur Midlander Male",
	2 => "Hyur Midlander Female",
	3 => "Hyur Highlander Male",
	4 => "Elezen Wildwood Male",
	5 => "Elezen Wildwood Female",
	6 => "Elezen Duskwight Male",
	7 => "Elezen Duskwight Female",
	8 => "Lalafell Plainsfolk Male",
	9 => "Lalafell Plainsfolk Female",
	10 => "Lalafell Dunesfolk Male",
	11 => "Lalafell Dunesfolk Female",
	12 => "Miqo'te Seeker of the Sun Female",
	13 => "Miqo'te Keeper of the Moon Female",
	14 => "Roegadyn Sea Wolf Male",
	15 => "Roegadyn Hellsguard Male"
);

$g_guardians = array(
	1 => "Halone, the Fury",
	2 => "Menphina, the Lover",
	3 => "Thaliak, the Scholar",
	4 => "Nymeia, the Spinner",
	5 => "Llymlaen, the Navigator",
	6 => "Oschon, the Wanderer",
	7 => "Byregot, the Builder",
	8 => "Rhalgr, the Destroyer",
	9 => "Azeyma, the Warden",
	10 => "Nald'thal, the Traders",
	11 => "Nophica, the Matron",
	12 => "Althyk, the Keeper"
);

$g_allegiances = array(
	1 => "Limsa Lominsa",
	2 => "Gridania",
	3 => "Ul'dah",
);

/*
$g_htmlToDbFieldMapping = array(
	"characterName" => "name",
	"characterTribe" => "tribe",
	"characterSize" => "size",
	"characterVoice" => "voice",
	"characterSkinColor" => "skinColor",
	"characterHairStyle" => "hairStyle",
	"characterHairColor" => "hairColor",
	"characterHairOption" => "hairVariation",
	"characterEyeColor" => "eyeColor",
	"characterFaceType" => "faceType",
	"characterFaceBrow" => "faceEyebrows",
	"characterFaceEye" => "faceEyeShape",
	"characterFaceIris" => "faceIrisSize",
	"characterFaceNose" => "faceNose",
	"characterFaceMouth" => "faceMouth",
	"characterFaceJaw" => "faceJaw",
	"characterFaceCheek" => "faceCheek",
	"characterFaceOption1" => "faceOption1",
	"characterFaceOption2" => "faceOption2",
	"characterGuardian" => "guardian",
	"characterBirthMonth" => "birthMonth",
	"characterBirthDay" => "birthDay",
	"characterAllegiance" => "initialTown",
	"characterWeapon1" => "weapon1",
	"characterWeapon2" => "weapon2",
	"characterHeadGear" => "headGear",
	"characterBodyGear" => "bodyGear",
	"characterLegsGear" => "legsGear",
	"characterHandsGear" => "handsGear",
	"characterFeetGear" => "feetGear",
	"characterWaistGear" => "waistGear",
	"characterRightEarGear" => "rightEarGear",
	"characterLeftEarGear" => "leftEarGear",
	"characterRightFingerGear" => "rightFingerGear",
	"characterLeftFingerGear" => "leftFingerGear"
);
*/

$g_height = array(
	0 => "Shortest",
	1 => "Short",
	2 => "Average",
	3 => "Tall",
	4 => "Tallest"
);

$g_yesno = array(
	0 => "No",
	1 => "Yes"
);

$g_grandcompany = array(
	0 => "None",
	/* TODO: Find correct order for 1+ */
	1 => "Maelstrom", 
	2 => "Order of the Twin Adder ",
	3 => "Immortal Flames"
);

$g_profileMapping = array(
	"characterName" => "name",
	"characterCreationDate" => "creationDate",
	"characterIsLegacy" => "isLegacy",
	"characterPlayTime" => "playTime",
/*
	"characterPositionX" => "positionX",
	"characterPositionY" => "positionY",
	"characterPositionZ" => "positionZ",
	"characterPositionR" => "rotation",
	"characterCurrentZoneId" => "currentZoneId",
*/
	"characterGuardian" => "guardian",
	"characterBirthDay" => "birthDay",
	"characterBirthMonth" => "birthMonth",
	"characterAllegiance" => "initialTown",
	"characterTribe" => "tribe",
	"characterGcCurrent" => "gcCurrent",
	"characterGcLimsaRank" => "gcLimsaRank",
	"characterGcGridaniaRank" => "gcGridaniaRank",
	"characterGcUldahRank" => "gcUldahRank",
/*
	"characterCurrentTitle" => "currentTitle",
	"characterRestBonus" => "restBonus",
*/
	"characterAchievementPoints" => "achievementPoints",
);

$g_appearanceMapping = array(
/*
	"characterBaseId" => "baseId", // Basic appearance?
*/
	"characterSize" => "size",
	"characterVoice" => "voice",
	"characterSkinColor" => "skinColor",
	"characterHairStyle" => "hairStyle",
	"characterHairColor" => "hairColor",
	"characterHairHighlightColor" => "hairHighlightColor",
	"characterHairVariation" => "hairVariation",
	"characterEyeColor" => "eyeColor",
	"characterFaceType" => "faceType",
	"characterFaceBrow" => "faceEyebrows",
	"characterFaceEye" => "faceEyeShape",
	"characterFaceIris" => "faceIrisSize",
	"characterFaceNose" => "faceNose",
	"characterFaceMouth" => "faceMouth",	
	"characterFaceFeatures" => "faceFeatures",
	"characterFaceEars" => "ears",
	"characterFaceCharacteristics" => "characteristics",
	"characterFaceCharacteristicsColor" => "characteristicsColor"
);

$g_chocoboMapping = array(
	"characterHasChocobo" => "hasChocobo",
	"characterHasGoobbue" => "hasGoobbue",
	"characterChocoboAppearance" => "chocoboAppearance",
	"characterChocoboName" => "chocoboName"
);

$g_classLevels = array(
	"characterGla" => "gla",
	"characterPug" => "pug",
	"characterMrd" => "mrd",
	"characterLnc" => "lnc",
	"characterArc" => "arc",
	"characterCnj" => "cnj",
	"characterThm" => "thm",
	"characterCrp" => "crp",
	"characterBsm" => "bsm",
	"characterArm" => "arm",
	"characterGsm" => "gsm",
	"characterLtw" => "ltw",
	"characterWvr" => "wvr",
	"characterAlc" => "alc",
	"characterCul" => "cul",
	"characterMin" => "min",
	"characterBtn" => "btn",
	"characterFsh" => "fsh"
);

function SaveCharacter($databaseConnection, $htmlFieldMapping, $characterId)
{
	$characterInfo = array();
	foreach($htmlFieldMapping as $htmlFieldName => $dbFieldName)
	{
		$characterInfo[$dbFieldName] = $_POST[$htmlFieldName];
	}
	UpdateCharacterInfo($databaseConnection, $characterId, $characterInfo);
}

function GenerateTextField($characterInfo, $htmlFieldMapping, $htmlFieldName, $fieldMaxLength = null)
{
	$inputMaxLength = ($fieldMaxLength === null) ? "" : sprintf("maxlength=\"%d\"", $fieldMaxLength);
	return sprintf("<input id=\"%s\" name=\"%s\" type=\"text\" value=\"%s\" %s readonly=\"readonly\" />",
		$htmlFieldName, $htmlFieldName, $characterInfo[$htmlFieldMapping[$htmlFieldName]], $inputMaxLength);
}

function GenerateSelectField($characterInfo, $htmlFieldMapping, $htmlFieldOptions, $htmlFieldName)
{
	$dbFieldName = $htmlFieldMapping[$htmlFieldName];
	$htmlText = sprintf("<select id=\"%s\" name=\"%s\">\n",
		$htmlFieldName, $htmlFieldName);
	foreach($htmlFieldOptions as $optionId => $optionName)
	{
		$htmlText .= sprintf("<option value=\"%d\" %s>%s</option>\n", 
				$optionId, 
				($optionId === (int)$characterInfo[$dbFieldName]) ? "selected" : "",
				$optionName);
	}
	$htmlText .= "</select>\n";
	return $htmlText;
}

if(isset($_POST["cancel"]))
{
	header("Location: control_panel.php");
	exit;
}

if(isset($_POST["save"]))
{
	SaveCharacter($g_databaseConnection, $g_htmlToDbFieldMapping, $g_characterId);
	header("Location: control_panel.php");
	exit;
}

try
{
	$g_characterInfo = GetCharacterInfo($g_databaseConnection, $g_userId, $g_characterId);
	$g_characterAppearance = GetCharacterAppearance($g_databaseConnection, $g_userId, $g_characterId);
/*	$g_characterChocobo = GetCharacterChocobo($g_databaseConnection, $g_userId, $g_characterId); */
	$g_characterClassLevels = GetCharacterClassLevels($g_databaseConnection, $g_userId, $g_characterId);
}
catch(Exception $e)
{
	header("Location: control_panel.php");
	exit;
}

?>
<!DOCTYPE HTML>
<html xmlns="http://www.w3.org/1999/xhtml">
	<head>
		<meta charset="UTF-8">
		<title>Character Info</title>
		<link rel="stylesheet" type="text/css" href="css/reset.css" />
		<link rel="stylesheet" type="text/css" href="css/global.css" />
		<script type="application/ecmascript">
			var weaponPresets = <?php echo require_once("presets_weapon.json"); ?>; 
			var armorPresets = <?php echo require_once("presets_armor.json"); ?>;

			function loadPresetsInSelect(presets, selectName)
			{
				var select = document.getElementById(selectName);
				for(var presetId in presets)
				{
					var el = document.createElement("option");
					var preset = presets[presetId];
					el.textContent = preset.name;
					el.value = presetId;
					select.appendChild(el);
				}
			}
			
			window.onload = function() 
			{
				loadPresetsInSelect(weaponPresets, "weaponPresets");
				loadPresetsInSelect(armorPresets, "armorPresets");
			}
			
			function byteArrayToString(byteArray)
			{
				var i, str = '';
				for(i = 0; i < byteArray.length; i++) 
				{
					str += String.fromCharCode(byteArray[i]);
				}
				return str;
			}
			
			function decodeCharacterFile(inputArrayBuffer)
			{
				var outputArrayBuffer = new ArrayBuffer(inputArrayBuffer.byteLength);
				var inputDataView = new DataView(inputArrayBuffer);
				var outputDataView = new DataView(outputArrayBuffer);
				for(var i = 0; i < inputDataView.byteLength; i++)
				{
					outputDataView.setUint8(i, inputDataView.getUint8(i) ^ 0x73);
				}
				return outputArrayBuffer;
			}
			
			function getCharacterAttributesFromString(characterFileString)
			{
				var lineArray = characterFileString.split('\n');
				lineArray = lineArray.filter(function(str) { return str != ''; });
				var characterAttributes = [];
				for(var i = 0; i < lineArray.length; i++)
				{
					var attributeLine = lineArray[i];
					attributeItems = attributeLine.split(',');
					characterAttributes.push( 
						{ 
							"name" : attributeItems[0].trim(),
							"value" : attributeItems[3].trim()
						}
					);
				}
				return characterAttributes;
			}
			
			function getCharacterAttributeValue(attributes, attributeName)
			{
				for(var i = 0; i < attributes.length; i++)
				{
					var attribute = attributes[i];
					if(attribute.name === attributeName)
					{
						return attribute.value;
					}
				}
				return undefined;
			}
			
			function onImportAppearanceFileReaderLoad(evt)
			{
				var decodedCharacterFileArray = decodeCharacterFile(evt.target.result);
				var decodedCharacterFileString = byteArrayToString(new Uint8Array(decodedCharacterFileArray));
				var characterAttributes = getCharacterAttributesFromString(decodedCharacterFileString);
				var fieldAssociations = 
				[
					[ 'characterSize',			'appearancetype_size' ],
					[ 'characterVoice', 		'appearancetype_voice' ],
					[ 'characterSkinColor',		'appearancetype_skin' ],
					[ 'characterHairStyle',		'appearancetype_hairstyle' ],
					
					[ 'characterHairColor',		'appearancetype_haircolor' ],
					[ 'characterHairOption',	'appearancetype_hairoption2' ],
					[ 'characterEyeColor', 		'appearancetype_eyecolor' ],
					[ 'characterFaceType', 		'appearancetype_facetype' ],
					
					[ 'characterFaceBrow',		'appearancetype_facebrow' ],
					[ 'characterFaceEye',		'appearancetype_faceeye' ],
					[ 'characterFaceIris',		'appearancetype_faceiris' ],
					[ 'characterFaceNose', 		'appearancetype_facenose' ],
					
					[ 'characterFaceMouth', 	'appearancetype_facemouth' ],
					[ 'characterFaceJaw', 		'appearancetype_facejaw_special' ],
					[ 'characterFaceCheek', 	'appearancetype_facecheek' ],
					[ 'characterFaceOption1', 	'appearancetype_faceoption1' ],
					
					[ 'characterFaceOption2', 	'appearancetype_faceoption2' ],
				];
				var characterTribe = getCharacterAttributeValue(characterAttributes, "rsc_tribe");
				var characterTribeSelect = document.getElementById('characterTribe');
				for(var i = 0; i < characterTribeSelect.length; i++)
				{
					var characterTribeSelectItem = characterTribeSelect[i];
					characterTribeSelectItem.selected = characterTribeSelectItem.value === characterTribe;
				}
				for(var i = 0; i < fieldAssociations.length; i++)
				{
					var fieldAssociation = fieldAssociations[i];
					var attributeValue = getCharacterAttributeValue(characterAttributes, fieldAssociation[1]);
					document.getElementById(fieldAssociation[0]).value = attributeValue;
				}
			}
			
			function importAppearanceFromFile(evt)
			{
				var file = evt.target.files[0];
				var fileReader = new FileReader();
				fileReader.readAsArrayBuffer(file);
				fileReader.onload = onImportAppearanceFileReaderLoad;
			}
			
			function onEquipWeaponPreset()
			{
				var select = document.getElementById("weaponPresets");
				
				var weapon1Field = document.getElementById("characterWeapon1");
				var weapon2Field = document.getElementById("characterWeapon2");

				var preset = weaponPresets[select.value];
				weapon1Field.value = preset.weapon1;
				weapon2Field.value = preset.weapon2;
			}
			
			function onEquipArmorPreset()
			{
				var select = document.getElementById("armorPresets");

				var headGearField = document.getElementById("characterHeadGear");
				var bodyGearField = document.getElementById("characterBodyGear");
				var legsGearField = document.getElementById("characterLegsGear");
				var handsGearField = document.getElementById("characterHandsGear");
				var feetGearField = document.getElementById("characterFeetGear");
				var waistGearField = document.getElementById("characterWaistGear");
				
				var preset = armorPresets[select.value];
				headGearField.value = preset.headGear;
				bodyGearField.value = preset.bodyGear;
				legsGearField.value = preset.legsGear;
				handsGearField.value = preset.handsGear;
				feetGearField.value = preset.feetGear;
				waistGearField.value = preset.waistGear;
			}
		
			function toggleDisplay(elementName)
			{
				var element = document.getElementById(elementName);
				if(element.style.display === 'none')
				{
					element.style.display = '';
				}
				else
				{
					element.style.display = 'none';
				}
			}
			
		</script>
	</head>
	<body>
		<?php include("header.php"); ?>
		<?php include("control_panel_header.php"); ?>
		<div class="edit">
			<h2>Character Info (<a href="#" onclick="toggleDisplay('guideDiv');">Help</a>)</h2>
			<div id="guideDiv" style="background-color: white; display: none;">
				<h3>Character Appearance Notes:</h3>
				<p style="text-align: left">
					Any value that is a bare number without any other description before or after it does 
					not correlate to the values selected in the ingame character creator.
				</p>
				<!--
				<p style="text-align: left">
					All values here are editable, so change them at your own risk. Just keep in mind that
					you can always import an appearance from a character creation data file and equip presetted
					equipment to reset your character.
				</p>
				<h3>Import Appearance</h3>
				<p style="text-align: left">
					Use this to import a character creation data file. Those files
					are created by the client when saving character creation data in the character creation 
					mode, just before selecting the server on which the character will be created. They are usually
					located in the "C:\Users\{Username}\Documents\My Games\FINAL FANTASY XIV\user\00000000" folder 
					and have a '.CMB' extension.
				</p>
				-->
			</div>
			<br />
			
			<form method="post" autocomplete="off">
				<table class="editForm">
					<tr>
						<th colspan="4">Profile</th>
					</tr>
					<tr>
						<td>Name:</td>
						<td>Legacy Character:</td>
						<td>Creation Date:</td>
						<td>Play Time:</td>
					</tr>
					<tr>
						<td><?php echo GenerateTextField($g_characterInfo, $g_profileMapping, "characterName", 20); ?></td>
						<td><?php echo GenerateSelectField($g_characterInfo, $g_profileMapping, $g_yesno, "characterIsLegacy"); ?></td>
						<td><?php echo GenerateTextField($g_characterInfo, $g_profileMapping, "characterCreationDate", 20); ?></td>
						<td><?php echo GenerateTextField($g_characterInfo, $g_profileMapping, "characterPlayTime"); ?></td>
					</tr>
					<tr>
						<td>Guardian:</td>
						<td>Birth Month:</td>
						<td>Birth Day:</td>
						<td>Allegiance:</td>
					</tr>
					<tr>
						<td><?php echo GenerateSelectField($g_characterInfo, $g_profileMapping, $g_guardians, "characterGuardian"); ?></td>
						<td><?php echo GenerateTextField($g_characterInfo, $g_profileMapping, "characterBirthMonth"); ?></td>
						<td><?php echo GenerateTextField($g_characterInfo, $g_profileMapping, "characterBirthDay"); ?></td>
						<td><?php echo GenerateSelectField($g_characterInfo, $g_profileMapping, $g_allegiances, "characterAllegiance"); ?></td>
					</tr>
					<tr>
						<td>Current GC:</td>
						<td>Maelstrom Rank:</td>
						<td>Twin Adder Rank:</td>
						<td>Immortal Flame Rank:</td>
					</tr>
					<tr>
						<td><?php echo GenerateSelectField($g_characterInfo, $g_profileMapping, $g_grandcompany, "characterGcCurrent"); ?></td>
						<td><?php echo GenerateTextField($g_characterInfo, $g_profileMapping, "characterGcLimsaRank"); ?></td>
						<td><?php echo GenerateTextField($g_characterInfo, $g_profileMapping, "characterGcGridaniaRank"); ?></td>
						<td><?php echo GenerateTextField($g_characterInfo, $g_profileMapping, "characterGcUldahRank"); ?></td>
					</tr>
					<!--
					<tr>
						<td>Chocobo Unlocked:</td>
						<td>Goobbue Unlocked:</td>
						<td>Chocobo Appearance:</td>
						<td>Chocobo Name:</td>
					</tr>
					<tr>
						<td><?php echo GenerateSelectField($g_characterChocobo, $g_chocoboMapping, $g_yesno, "characterHasChocobo"); ?></td>
						<td><?php echo GenerateSelectField($g_characterChocobo, $g_chocoboMapping, $g_yesno, "characterHasGoobbue"); ?></td>
						<td><?php echo GenerateTextField($g_characterChocobo, $g_chocoboMapping, "characterChocoboAppearance"); ?></td>
						<td><?php echo GenerateTextField($g_characterChocobo, $g_chocoboMapping, "characterChocoboName"); ?></td>
					</tr>
					-->
					<tr>
						<td>GLA:</td>
						<td>PUG:</td>
						<td>MRD:</td>
						<td>LNC:</td>
					</tr>
					<tr>
						<td><?php echo GenerateTextField($g_characterClassLevels, $g_classLevels, "characterGla"); ?></td>
						<td><?php echo GenerateTextField($g_characterClassLevels, $g_classLevels, "characterPug"); ?></td>
						<td><?php echo GenerateTextField($g_characterClassLevels, $g_classLevels, "characterMrd"); ?></td>
						<td><?php echo GenerateTextField($g_characterClassLevels, $g_classLevels, "characterLnc"); ?></td>
					</tr>
					<tr>
						<td>ARC:</td>
						<td>CNJ:</td>
						<td>THM:</td>
					</tr>
					<tr>
						<td><?php echo GenerateTextField($g_characterClassLevels, $g_classLevels, "characterArc"); ?></td>
						<td><?php echo GenerateTextField($g_characterClassLevels, $g_classLevels, "characterCnj"); ?></td>
						<td><?php echo GenerateTextField($g_characterClassLevels, $g_classLevels, "characterThm"); ?></td>
					</tr>
					<tr>
						<td>CRP:</td>
						<td>BSM:</td>
						<td>ARM:</td>
						<td>GSM:</td>
					</tr>
					<tr>
						<td><?php echo GenerateTextField($g_characterClassLevels, $g_classLevels, "characterCrp"); ?></td>
						<td><?php echo GenerateTextField($g_characterClassLevels, $g_classLevels, "characterBsm"); ?></td>
						<td><?php echo GenerateTextField($g_characterClassLevels, $g_classLevels, "characterArm"); ?></td>
						<td><?php echo GenerateTextField($g_characterClassLevels, $g_classLevels, "characterGsm"); ?></td>
					</tr>
					<tr>
						<td>LTW:</td>
						<td>WVR:</td>
						<td>ALC:</td>
						<td>CUL:</td>
					</tr>
					<tr>
						<td><?php echo GenerateTextField($g_characterClassLevels, $g_classLevels, "characterLtw"); ?></td>
						<td><?php echo GenerateTextField($g_characterClassLevels, $g_classLevels, "characterWvr"); ?></td>
						<td><?php echo GenerateTextField($g_characterClassLevels, $g_classLevels, "characterAlc"); ?></td>
						<td><?php echo GenerateTextField($g_characterClassLevels, $g_classLevels, "characterCul"); ?></td>
					</tr>
					<tr>
						<td>MIN:</td>
						<td>BTN:</td>
						<td>FSH:</td>
					</tr>
					<tr>
						<td><?php echo GenerateTextField($g_characterClassLevels, $g_classLevels, "characterMin"); ?></td>
						<td><?php echo GenerateTextField($g_characterClassLevels, $g_classLevels, "characterBtn"); ?></td>
						<td><?php echo GenerateTextField($g_characterClassLevels, $g_classLevels, "characterFsh"); ?></td>
					</tr>
				</table>
				<br />
				<hr />
				<table class="editForm">
					<tr>
						<th colspan="5">Appearance</th>
					</tr>
					<tr>
						<td colspan="2">Race/Tribe:</td>
						<td>Height:</td>
						<td>Voice:</td>
						<td>Skin Tone:</td>
					</tr>
					<tr>
						<td colspan="2"><?php echo GenerateSelectField($g_characterInfo, $g_profileMapping, $g_tribes, "characterTribe"); ?></td>
						<td><?php echo GenerateSelectField($g_characterAppearance, $g_appearanceMapping, $g_height, "characterSize"); ?></td>
						<td><?php echo GenerateTextField($g_characterAppearance, $g_appearanceMapping, "characterVoice"); ?></td>
						<td><?php echo GenerateTextField($g_characterAppearance, $g_appearanceMapping, "characterSkinColor"); ?></td>
					</tr>
					<tr>
						<td>Hairstyle:</td>
						<td>Variation:</td>
						<td>Hair Color:</td>
						<td>Highlights:</td>
					</tr>
					<tr>
						<td><?php echo GenerateTextField($g_characterAppearance, $g_appearanceMapping, "characterHairStyle"); ?></td>
						<td><?php echo GenerateTextField($g_characterAppearance, $g_appearanceMapping, "characterHairVariation"); ?></td>
						<td><?php echo GenerateTextField($g_characterAppearance, $g_appearanceMapping, "characterHairColor"); ?></td>
						<td><?php echo GenerateTextField($g_characterAppearance, $g_appearanceMapping, "characterHairHighlightColor"); ?></td>
					</tr>
					<tr>
						<td>Face Type:</td>
						<td>Eyebrows:</td>
						<td>Eye Shape:</td>
						<td>Iris Size:</td>
					</tr>
					<tr>
						<td>Eye Color:</td>
						<td>Nose:</td>
						<td>Face Mouth:</td>
						<td>Features:</td>						
					</tr>
					<tr>
						<td><?php echo GenerateTextField($g_characterAppearance, $g_appearanceMapping, "characterFaceType"); ?></td>
						<td><?php echo GenerateTextField($g_characterAppearance, $g_appearanceMapping, "characterFaceBrow"); ?></td>
						<td><?php echo GenerateTextField($g_characterAppearance, $g_appearanceMapping, "characterFaceEye"); ?></td>
						<td><?php echo GenerateTextField($g_characterAppearance, $g_appearanceMapping, "characterFaceIris"); ?></td>
					</tr>
					<tr>
						<td><?php echo GenerateTextField($g_characterAppearance, $g_appearanceMapping, "characterEyeColor"); ?></td>
						<td><?php echo GenerateTextField($g_characterAppearance, $g_appearanceMapping, "characterFaceNose"); ?></td>
						<td><?php echo GenerateTextField($g_characterAppearance, $g_appearanceMapping, "characterFaceMouth"); ?></td>
						<td><?php echo GenerateTextField($g_characterAppearance, $g_appearanceMapping, "characterFaceFeatures"); ?></td>
					</tr>
					<tr>						
						<td>Characteristic:</td>
						<td>Color:</td>
						<td>Ears:</td>
					</tr>
					<tr>
						<td><?php echo GenerateTextField($g_characterAppearance, $g_appearanceMapping, "characterFaceCharacteristics"); ?></td>
						<td><?php echo GenerateTextField($g_characterAppearance, $g_appearanceMapping, "characterFaceCharacteristicsColor"); ?></td>
						<td><?php echo GenerateTextField($g_characterAppearance, $g_appearanceMapping, "characterFaceEars"); ?></td>
					</tr>
					<!--
					<tr>
						<td></td>
					</tr>
					<tr>
						<td>
							<script>
								function onImportAppearanceButtonClick()
								{
									document.getElementById('importAppearance').click();
								}
							</script>
							<input type="file" id="importAppearance" style="display: none;">
							<button onclick="onImportAppearanceButtonClick(); return false;">Import Appearance</button>
							<script>
								document.getElementById('importAppearance').addEventListener('change', importAppearanceFromFile, false);
							</script>
						</td>
					</tr>
					-->
				</table>
				<br />
				<hr />
				<!--
				<table class="editForm">
					<tr>
						<th colspan="4">Gear</th>
					</tr>
					<tr>
						<td>Weapon 1:</td>
						<td>Weapon 2:</td>
						<td></td>
						<td></td>
					</tr>
					<tr>
						<td><?php echo GenerateTextField($g_characterInfo, $g_htmlToDbFieldMapping, "characterWeapon1"); ?></td>
						<td><?php echo GenerateTextField($g_characterInfo, $g_htmlToDbFieldMapping, "characterWeapon2"); ?></td>
						<td></td>
						<td></td>
					</tr>
					<tr>
						<td>Head Gear:</td>
						<td>Body Gear:</td>
						<td>Legs Gear:</td>
						<td>Hands Gear:</td>
					</tr>
					<tr>
						<td><?php echo GenerateTextField($g_characterInfo, $g_htmlToDbFieldMapping, "characterHeadGear"); ?></td>
						<td><?php echo GenerateTextField($g_characterInfo, $g_htmlToDbFieldMapping, "characterBodyGear"); ?></td>
						<td><?php echo GenerateTextField($g_characterInfo, $g_htmlToDbFieldMapping, "characterLegsGear"); ?></td>
						<td><?php echo GenerateTextField($g_characterInfo, $g_htmlToDbFieldMapping, "characterHandsGear"); ?></td>
					</tr>
					<tr>
						<td>Feet Gear:</td>
						<td>Waist Gear:</td>
						<td></td>
						<td></td>
					</tr>
					<tr>
						<td><?php echo GenerateTextField($g_characterInfo, $g_htmlToDbFieldMapping, "characterFeetGear"); ?></td>
						<td><?php echo GenerateTextField($g_characterInfo, $g_htmlToDbFieldMapping, "characterWaistGear"); ?></td>
						<td></td>
						<td></td>
					</tr>
					<tr>
						<td>Right Ear Gear:</td>
						<td>Left Ear Gear:</td>
						<td>Right Finger Gear:</td>
						<td>Left Finger Gear:</td>
					</tr>
					<tr>
						<td><?php echo GenerateTextField($g_characterInfo, $g_htmlToDbFieldMapping, "characterRightEarGear"); ?></td>
						<td><?php echo GenerateTextField($g_characterInfo, $g_htmlToDbFieldMapping, "characterLeftEarGear"); ?></td>
						<td><?php echo GenerateTextField($g_characterInfo, $g_htmlToDbFieldMapping, "characterRightFingerGear"); ?></td>
						<td><?php echo GenerateTextField($g_characterInfo, $g_htmlToDbFieldMapping, "characterLeftFingerGear"); ?></td>
					</tr>
					<tr>
						<td colspan="2">Weapon Presets:</td>
						<td colspan="2">Armor Presets:</td>
					</tr>
					<tr>
						<td colspan="2">
							<select id="weaponPresets"></select>
							<button onclick="onEquipWeaponPreset(); return false;">Equip</button>
						</td>
						<td colspan="2">
							<select id="armorPresets"></select>
							<button onclick="onEquipArmorPreset(); return false;">Equip</button>
						</td>
					</tr>
				</table>
				<br />
				<hr />
				-->
				<!--
				<table class="infoForm">
					<tr>
						<td>
							<input type="submit" name="save" value="Save" />
							<input type="submit" name="cancel" value="Cancel" />
						</td>
					</tr>
				</table>
				-->
			</form>
		</div>
		<div>
		</div>
	</body>
</html>