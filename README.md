# Würfelware - Passphrases for KeePass
A KeePass password generator. Generates passphrases within KeePass. In tribute to https://xkcd.com/936/.

# What does Würfelware do?
It registers as a password generator in KeePass and generates passphrases. Passphrases are passwords consisting of multiple words, thus they are "phrases".

The words used to make passphrases are taken from a text file. Words are randomly chosen. The number of words can be configured. 

The resulting passphrases look like those:

* irin schade viper pass sollst
* beraet fauna schere sampan herauf
* ans teller gerben droben lullt
* bau drehen flakon gnu gewann
* kohle kasack unten festes kardex
* sechse dose adlig pokal lage

Hard to guess but easy to memorize or type.

# Installation
Copy the plugin files to the KeePass plugins folder. I'd recommend creating a sub-folder. So the folder structure will look like *KeePass\Plugins\Wuerfelware*.

Plugin files that need to be copied to *KeePass\Plugins\Wuerfelware*:
* PwGenWuerfelware.dll
* Newtonsoft.Json.dll
* wuerfelware.txt

If everything is ok you should have a new password generator *Würfelware* available in KeePass.

# Changing the word list
Only one word list is supported - it's in the *wuerfelware.txt* file. You can change the content of this file as you wish.

Whitespaces and line breaks are used as word delimiters, so a text file containing *correct battery horse staple* will be read as a word list containing 4 words.

Any numbers (like *12435*) in this file will be ignored which is handy if you use an existing word list (like the initial *wuerfelware.txt*). Just leave the numbers in, they do no harm.

# Supported Languages
The plugin language is English.

The initial word list *wuerfelware.txt* contains German words. You can replace the file content with words in any language.

# Word lists
The initial word list *wuerfelware.txt* was downloaded from here:
http://world.std.com/~reinhold/diceware_german.txt (*A German word list provided by Benjamin Tenne under the terms of the GNU General Public License.*)

I did not alter it, just renamed it to make the file name more generic.

There are more files in different languages available, you can get an overview here:
http://world.std.com/~reinhold/diceware.html

Of cource, many more word lists can be found. A good keyword to google for is *Diceware*.
