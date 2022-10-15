# xPug Companion App
The xPug Companion App is a mobile & desktop application where users can play with & dress up their own virutal pet.

There are 3 main features of the app in its current state:
  - Menu
  - Dress (Wardrobe)
  - Game (Minigame Hub)
  
![xPug Video Game Promo Small](https://user-images.githubusercontent.com/102563881/195963451-828d1269-e5ef-4fd1-888a-eda3b88c9ce9.png)
  
# Wardrobe Explanation
Dress up & pose your pug with cosmetics unlocked by (not final):
  - Owning specific xPug NFTs
  - Playing minigames
  - Purchasing from the Shop
  
![dress_1](https://user-images.githubusercontent.com/102563881/195961935-5e8f7841-eec1-4390-adb0-c71b77ba57b1.png)
![dress_3_alt2](https://user-images.githubusercontent.com/102563881/195961936-57154156-d68b-48ba-af7e-27e79bd5a29d.png)
![dress_4](https://user-images.githubusercontent.com/102563881/195961937-2784c9c8-8c1f-4482-9e9c-e564798b4fdc.png)
![dress_5](https://user-images.githubusercontent.com/102563881/195961939-a1f129e6-67aa-4f6c-a562-6fceb316ec13.png)

# xPug Battle Explanation
 - Play against endless randomly generated opponents
 - Adapt your strategies of playing as your opponents adapt to you
 - Upgrade your attacks by defeating bosses to get stronger
 - Unlock cosmetics by playing
 - Easy to learn, difficult to master
 
 Gameplay:
 https://youtu.be/DZymCripGcM
 
![battle1](https://user-images.githubusercontent.com/102563881/195962779-968ec8a8-bc4b-4da4-ba63-266fe58ad3d4.png)
![battle2](https://user-images.githubusercontent.com/102563881/195962793-e248a418-6e2f-487d-8111-0f1f1cadb0dc.png)
![battle3](https://user-images.githubusercontent.com/102563881/195962797-dcc4bff3-1f2e-40a0-9518-9716ab641714.png)
 
 # Menu Explanation
  - Hub to the app
  - Can access the wardrobe (Dress)
  - Can access minigames (Game)
  - Planned features such as Shop and Play

  ![menu1](https://user-images.githubusercontent.com/102563881/195962827-291839f5-eb14-4d83-a80b-4d789f8ab6c1.png)
  
# Basic Code Explanation
Using xPug Battle as an example, path can be found at [Assets \ ~Minigame_PugJitsu \ !Scripts]

[Attack_Controller.cs]
Main class on the player responsible for their attributes, attacks, taking damage, death

[Enemy_Attack_Controller.cs]
Inherits Attack_Controller to do essentially the same thing as Attack_Controller, but adds behaviors for AI + other small things 
