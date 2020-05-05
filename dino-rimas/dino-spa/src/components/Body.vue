<template>
    <div class="lk-body">
        <div class="server-select row justify-content-center">
            <div class="button" :class="{'button-active': server == 0}" @click="emit('serverSelect', 0)">Сервер 1</div>
            <div class="button" :class="{'button-active': server == 1}" @click="emit('serverSelect', 1)">Сервер 2</div>
        </div>

      
        
        <div class="lk-options" :class="{'lk-options-enable': options.enabled}">
            <div class="lk-options-map">
                <img src="/img/map_0.jpg" alt="map">                
                <div 
                    v-for="(point, index) in points" 
                    :key="index" class="lk-options-pos" 
                    @click="emit('pos', options.id, index);" 
                    :style="{'left' : getLeft(point[0]), 'top': getTop(point[1])}"
                ></div>
                <div class="currentPos" :style="{'left': options.pos.left, 'top': options.pos.top}"><img src="/img/logo.png" /></div>
            </div>
            <div class="lk-options-buttons">
                <div class="lk-options-name" >{{options.name}}</div>
                <div class="button" v-if="options.active" @click="emit('desactivate', options.id);">Деактивировать</div>
                <div v-else>
                    <div class="button"  @click="emit('activate', options.id);">Активировать</div>
                    <div class="button" @click="emit('sex', options.id);">Сделать самкой</div>
                    <div class="button" @click="emit('grow', options.id);">Прокачать</div>
                </div>        
                <div class="button" @click="emit('delete', options.id);">Удалить</div>        
                <div class="button" @click="options.enabled = false">Назад</div>
                <div>
                    <h3>Стоимость:</h3>
                    <p>Сменить пол Survival - <span class="lk-options-price">{{price.sex}} DC</span></p>
                    <p>Сменить пол Progression - <span class="lk-options-price">{{price.sex * 2}} DC</span></p>
                    <p>Прокачка - <span class="lk-options-price">{{price.grow}} DC</span></p>
                    <p>Телепортировать - <span class="lk-options-price">{{price.position}} DC</span></p>
                </div>
            </div>
               
            <div class="stadies">
                <div class="stage" v-for="(stage, index) in stadies" :key="index" :class="{'stage-active': stadyActive(stage)}">
                    <div class="stage-label">{{stage}}</div>
                </div>
            </div>
        </div>
        <div class="lk-inventory row align-items-center" v-show="!options.enabled">

            <div class="lk-inventory-item"
                v-for="(dino, index) in inventory" :key="index"
                :class="{'lk-inventory-activeted': dino && dino.active}"
                :title="dino ? dino.name + (dino.active ? ' : активен' : ' : не активен') : ''"
                @click="showOptions(dino)"
            >
                <div v-if="dino">
                    <div class="lk-inventory-img">
                        <img :src="`/img/Dinos/${dino.characterClass.toLowerCase()}.png`" :alt="dino.name">
                    </div>               
                    <div class="lk-inventory-tittle">{{dino.name}}</div>
                    <img class="lk-inventory-sex" :src="dino.bGender ? '/img/female.svg' : '/img/male.svg'" >
                    <div class="lk-inventory-growth">Рост: {{Math.round(dino.growth * 100)}}</div>
                </div>
                <div v-else>
                    <div class="lk-inventory-tittle">Слот</div>
                    <div class="lk-inventory-tittle">свободен</div>
                </div>
            </div>            
            <div class="lk-inventory-item lk-inventory-add" title="Добавить слот" @click="emit('slot');">+</div>
        </div>
    </div>
</template>

<script>
export default {
    props:['inventory', 'price','server'],
    data() {
        return {
            options:{
                id: -1,
                name: "",
                enabled: false,
                active: false,
                className: "",
                pos: {
                    left: 0,
                    top: 0
                }
            },
            points: [
                [-462282.156, -54966.359, -73247.383],
                [-517779.062 ,128792.383 ,-70784.594],
                [-237888.953 ,363150.5 ,-69253.328],
                [484007.688 ,195747.594 ,-72550.602],
                [-21750.398 ,85635.734 ,-68614.961],
                [46536.098 ,-186975.156 ,-65410.535],
                [-169665.875 ,-585717.5 ,-72682.609],

                [-588464.75,  -229543.391, -32074.162],
                [-395896.844, -269987.312, -66561.102],
                [-191814.516, -373199.594, -40851.805],
                [13567.566, -404817.188, -42980.176],
                [107723.289, 283723.094, -40183.656],
                [-205974.016, -30537.402, -64633.398],
                [-410652.125, 471226.312, -29178.383],
                [-324573.219, 151895.031, -65705.188],
                [303773.5, -134582.484, -24480.203]   
            ],
            kof:  0.00026,
            stadies:{
                1: "Hatch",                    
                2: "Juv",                    
                3: "Sub",                    
                4: "Adult"
            }
        }
    },
    methods: {
        showOptions(dino){
            if(!dino) return;
            this.options.id = dino.id;            
            this.options.name = dino.name;
            this.options.active = dino.active;
            this.options.enabled = true;
            this.options.className = dino.characterClass;
            this.getDinoPos(dino);          
        },
        getLeft(val){
            // window.console.log(val)
            // return 225 + 0 + 'px';
            return (225 + val * this.kof) + 'px';
        },
        getTop(val){
            //window.console.log(val)
            //return 225 + 0 + 'px';
            return (215 + val * this.kof) + 'px';
        },
        getDinoPos(dino){
            const temp = dino.location_Isle_V3.split(" ");
            this.options.pos.left = this.getLeft(parseFloat(temp[0].split("=")[1]));
            this.options.pos.top = this.getTop(parseFloat(temp[1].split("=")[1]));
            //window.console.log(JSON.stringify(pos));
        },
        emit(action, data_1, data_2){
            this.$emit('onAction', action, data_1, data_2);
            this.options.enabled = false;
        },
        stadyActive(className){
            return this.options.className.indexOf(className) != -1;
        }
    }
}
</script>

<style lang="scss">
@import '../colors';
.lk{
    &-body{
        color: $clr_1;
        padding: 25px;
        overflow: hidden;        
    }
    &-inventory{
        margin: 0 25px;
        color: #000;
        &-add{
            font-size: 150px;
            text-align: center;
        }
        &-item{
            background-color: rgba($clr_2, .5);
            width: 150px;
            height: 200px;
            border-radius: 5px;
            box-shadow: 15px 15px 10px 1px $clr_2;
            border: 1px solid $clr_3;
            padding: 15px;
            margin: 15px;
            position: relative;
            color: $clr_1;
            &:hover{
                cursor: pointer;
                transform: scale(1.05);
            }
        }
        &-img{
            img{
                width: 100%;
            }
        }
        &-tittle{
            text-align: center;
            margin-top: 15px;
        }
        &-activeted{            
            border: 2px solid $clr_4;
            box-shadow: 0 0 12px 1px $clr_4;
        }
        &-sex{
            width: 24px;
            height: 24px;
            position: absolute;
            right: 5px;
            bottom: 5px;
        }
        &-growth{
            position: absolute;
            font-weight: bold;
            right: 35px;
            bottom: 5px;
        }
    }
    &-options{            
        border-radius: 5px;
        box-shadow: 5px 5px 10px 1px $clr_2;
        border: 1px solid $clr_3;
        width: 95%;
        height: 580px;
        position: absolute;
        top: 2.5%;
        left: 2.5%;
        background-color: rgba($clr_2, .8);
        transform: translateY(-105%);
        transition: all .3s;        
        display: flex;
        justify-content: space-around;
        align-items: center;
        flex-wrap: wrap;
        &-container{
            height: 100%;
            width: 100%;
            justify-content: space-around;
            align-items: center;
            position: relative;
            padding: 15px;   
        }
        &-enable{                
            transform: translateY(0);
        }
        &-map{
            width: 450px;
            height: 450px;
            position: relative;
            img{
                width: 100%;
                height: 100%;
            }
        }
        &-buttons{
            min-width: 25%;
            .button{
                margin: 15px;
                width: 100%;
                text-align: center;
            }
        }
        &-name{
            text-align: center;
            text-transform: uppercase;
            color: $clr_4;
            font-weight: bold;
            font-size: 1.1rem;
        }
        &-price{
            color: $clr_4;
            font-weight: bold;
        }
        h3{
            margin-bottom: 10px;
        }
        p{
            margin-bottom: 5px;
        }
        &-pos{
            position: absolute;
            top: 0;
            left: 0;
            width: 15px;
            height: 15px;
            border-radius: 50%;
            box-shadow: 0 0 7px 1px $clr_2;
            background-color: $clr_4;
            border: 1px solid $clr_3;
            &:hover{
                transform: scale(1.2);
                cursor: pointer;
                background-color: #fff;
            }
        }
    }
    .server-select{
        .button{
            margin-right: 10px;
            &-active{
                background-color: $clr_4;
                color: $clr_3;
                font-weight: bold;
            }
        }
    }
    .stadies{
        width: 90%;
        height: 1vh;
        background-color: $clr_3;
        border-radius: 10px;
        display: flex;
        justify-content: space-between;
        margin: 4vh 0 0 0;
        .stage{
            width: 2vh;
            height: 2vh;
            border-radius: 50%;
            background-color: $clr_4;
            border: 2px solid $clr_3;
            margin-top: -.5vh;
            &-label{
                margin-top: -3vh;
                margin-left: -1vh;
                text-align: center;
            }
            &-active{                
                color: $clr_4;
                box-shadow: 0 0 1vh 0 $clr_4;            
            }
        }
    }
   
    .currentPos{
        position: absolute;
        width: 2.5vh;
        height: 2.5vh;
        perspective: 300px;
        img{
            animation-duration: .8s;
            animation-name: logo-rotate;
            animation-iteration-count: infinite;
            animation-timing-function: linear;
        }
    }
    @keyframes logo-rotate {
        0% {
            transform: rotateY(0deg);
        }

        100%{
            transform: rotateY(180deg);
        }

        // 100% {       
        //     transform: rotateY(180deg);
        // }
    }
}
</style>