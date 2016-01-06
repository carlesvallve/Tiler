# Asset Bundle Client
The client side is built around the AssetLoader class
This class provides a simple way to register and load assets.
The instantiation is done automaticaly.

Every prefab has his own callback to add it to the scene when it's loaded.


# Events
To track the loading status, there are 3 main events:
* OnBegin
* OnComplete
* OnError

This allow to easily create loading screen and create prefabs through code dynamically.


# Example
// Get the singleton instance
AssetLoader assetLoader = AssetLoader.instance;


// Register a list of assets
assetLoader.AddAsset("assets/views/home", "Background", (GameObject obj) => {
	obj.transform.SetParent(gameContainer.LayerBackground.transform, false);
});

assetLoader.AddAsset("assets/common/audio", "Menu_BGM_loop", (AudioClip audio) => {
	BGM_loop = audio;
});


// Global Events
assetLoader.OnBegin += () => {
	gameContainer.ShowLoading(true);
};

assetLoader.OnError += (error, name) => {
	Debug.Log(name + " - " + error);
};

assetLoader.OnComplete += () => {
	Init();
	gameContainer.ShowLoading(false);
};

// Start loading the assets
assetLoader.Load();