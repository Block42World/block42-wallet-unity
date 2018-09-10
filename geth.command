nohup ./geth --datadir chaindata --identity "Client_1" --port 30303 --rpc --rpcport 8142 --rpccorsdomain "*" --rpcapi "db,eth,net,personal,admin,miner,web3" --networkid 8100442 --nodiscover --debug console --mine 2>> eth.log &


./geth --datadir chaindata --identity "Client_1" --port 30303 --rpc --rpcport 8142 --rpccorsdomain "*" --rpcapi "db,eth,net,personal,admin,miner,web3" --networkid 8100442 --nodiscover --debug --mine

90000000000000000000