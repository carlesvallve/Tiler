using UnityEngine;
using System.Collections;

public class MarkovNameGenerator : MonoBehaviour {

/*// Returns the source text
function get() { return document.getElementById('in').value; }
 
// Clears the source textbox
function clear() { document.getElementById('in').value = ''; }
 
// Writes to the output textbox
function set(v) { document.getElementById('out').value = v; }
 
// Holds the state information
var cache = {
    '_START': []
};
 
document.getElementById('add').onclick = function () {
    // Get the source text and split it into words
    var text = get().split(/\s+/g);
    
    if (!text.length)
        return;
 
    document.getElementById('generate').disabled = false;
 
    // Add it to the start node's list of possibility
    cache['_START'].push(text[0]);
    
    // Now go through each word and add it to the previous word's node
    for (var i = 0; i < text.length - 1; i++) {
        if (!cache[text[i]])
            cache[text[i]] = [];
        cache[text[i]].push(text[i + 1]);
        
        // If it's the beginning of a sentence, add the next word to the start node too
        if (text[i].match(/\.$/))
            cache['_START'].push(text[i + 1]);
    }
    clear();
};
 
document.getElementById('generate').onclick = function () {
    // Start with the root node
    var currentWord = '_START';
    var str = '';
    
    // Generate 300 words of text
    for (var i = 0; i < 300; i++) {
        
        // Follow a random node, append it to the string, and move to that node
        var rand = Math.floor(Math.random() * cache[currentWord].length);
        str += cache[currentWord][rand];
        
        // No more nodes to follow? Start again. (Add a period to make things look better.)
        if (!cache[cache[currentWord][rand]]) {
            currentWord = '_START';
            if (!cache[currentWord][rand].match(/\.$/))
                str += '. ';
            else
                str += ' ';
        } else {
            currentWord = cache[currentWord][rand];
            str += ' ';
        }
    }
    set(str);*/
}
