# Würfelware - Passphrases for Keepass
A Keepass password generator. Generates passphrases within Keypass. In tribute to https://xkcd.com/936/.

# What does Würfelware do?
It registers as a password generator in Keepass and generates passphrases. Passphrases are passwords consisting of multiple words, thus they are "phrases".

The words used to make passphrases are taken from a text file. Words are randomly chosen. The number of words can be configured. 

The resulting passphrases look like those:

irin schade viper pass sollst
beraet fauna schere sampan herauf
ans teller gerben droben lullt
bau drehen flakon gnu gewann
kohle kasack unten festes kardex
sechse dose adlig pokal lage

Hard to guess but easy to memorize or type.

# Installation
Copy the plugin files to the Keepass plugins folder. I'd recommend creating a sub-folder. So the folder structure will look like *KeePass\Plugins\Wuerfelware*.

Plugin files that need to be copied to *KeePass\Plugins\Wuerfelware*:
* PwGenWuerfelware.dll
* Newtonsoft.Json.dll
* wuerfelware.txt

If everything is ok you should have a new password generator *Würfelware* available in Keepass.

# Changing the word list
Only one word list is supported - it's in the *wuerfelware.txt* file. You can change the content of this file as you wish.

Whitespaces and line breaks are used as word delimiters, so *correct battery horse staple* will be read as a word list containing 4 words.

Any numbers in this list will be ignored which is handy if you copy an existing word list (like the initial *wuerfelware.txt*). Just leave the numbers in, they do no harm.