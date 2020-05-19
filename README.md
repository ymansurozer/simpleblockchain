# Simple Blockchain Data Structure with C#

This program creates a simple blockchain data structure with strings provided by the user as transactions. It creates a blockchain with a genesis node, which mines blocks and appends them into the chain.

## Features
* Creates a blockchain with given difficulty (*i.e.* number of zeroes at the end of the hash)
* Receives strings from the user as transactions
* Groups transactions into blocks and calculates a valid hash (*i.e.* mines blocks)
* Validatse the proof and the block and appends it to the chain
* Displays the blockchain data (*i.e.* block explorer)

I have deliberately kept the algorithm simple and left out key management (*i.e.* transactions can only contain strings), P2P, the longest chain rule, a proper validation mechanism (broadcasting and receiving validations), and block reward. The purpose is to focus on the structure in which blocks connect to each other in a blockchain.

## Built with
C#

## Contributing
Open to contributions.

## Author
Yusuf Mansur Özer

## License
Licensed under GNU Affero General Public License v3.0. See the [LICENSE.md](https://github.com/ymansurozer/simpleblockchain/blob/master/LICENSE) file for details.
