const fs = require('fs');

function getCoordinates(gcode){
    const allContents = fs.readFileSync(gcode, 'utf-8');
    var allInstructions = [];
    var lastZ = 0;
    var lastX = 0;
    var lastY = 0;
    allContents.split(/\r?\n/).forEach((line) => {
        const lineWithoutComment = line.split(';')[0].trim();
        var instructions = [0,lastY,lastZ,lastX];
        if(lineWithoutComment.length != 0){
            const instruction = lineWithoutComment.split(' ');
            switch(instruction[0]){
                case "G0":
                    instructions[0] = 0;
                    for(let i = 1; i < instruction.length; i++){
                        switch(instruction[i].charAt(0)){
                            case 'X':
                                // Set # to instructions[3]
                                instructions[3] = parseFloat(instruction[i].split('X')[1]);
                                lastX = instructions[3];
                                break;
                            case 'Y':
                                // Set # to instructions[1]
                                instructions[1] = parseFloat(instruction[i].split('Y')[1]);
                                lastY = instructions[1];
                                break;
                            case 'Z':
                                // Set # to instructions[2]
                                instructions[2] = parseFloat(instruction[i].split('Z')[1]);
                                lastZ = instructions[2];
                                break;
                            default:
                                break;
                        }
                    }
                    allInstructions.push(instructions);
                    break;
                case "G1":
                    instructions[0] = 1;
                    for(let i = 1; i < instruction.length; i++){
                        switch(instruction[i].charAt(0)){
                            case 'X':
                                // Set # to instructions[3]
                                instructions[3] = parseFloat(instruction[i].split('X')[1]);
                                lastX = instructions[3];
                                break;
                            case 'Y':
                                // Set # to instructions[1]
                                instructions[1] = parseFloat(instruction[i].split('Y')[1]);
                                lastY = instructions[1];
                                break;
                            case 'Z':
                                // Set # to instructions[2]
                                instructions[2] = parseFloat(instruction[i].split('Z')[1]);
                                lastZ = instructions[2];
                                break;
                            default:
                                break;
                        }
                    }
                    allInstructions.push(instructions);
                    break;
                default:
                    break;
            }
        }
    });
    return allInstructions;
}


function writeCoordinatesToFile(coordinates, filePath) {
    let text = coordinates.map(next => next.join(' ')).join('\n');
    fs.writeFileSync(filePath, text);
    console.log('Coordinates written to ' + filePath);
}

let coordinates = getCoordinates("Part.gcode"); // Change this to match input file
let outputFilePath = "Part.txt";
writeCoordinatesToFile(coordinates, outputFilePath);