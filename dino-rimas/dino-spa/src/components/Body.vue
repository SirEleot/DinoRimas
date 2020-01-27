<template>
    <div class="lk-body">
        <div class="server-select row justify-content-center">
            <div class="button" :class="{'button-active': server == 0}" @click="$emit('onAction', 'serverSelect', 0)">Сервер 1</div>
            <div class="button" :class="{'button-active': server == 1}" @click="$emit('onAction', 'serverSelect', 1)">Сервер 2</div>
        </div>
        <div class="lk-options" :class="{'lk-options-enable': options.enabled}">
            <div class="lk-options-map">
                <img src="/img/map.jpg" alt="map">
                <div class="lk-options-pos" @click="$emit('onAction', 'pos', options.id, 0); options.enabled = false;"></div>
                <div class="lk-options-pos" @click="$emit('onAction', 'pos', options.id, 1); options.enabled = false;"></div>
                <div class="lk-options-pos" @click="$emit('onAction', 'pos', options.id, 2); options.enabled = false;"></div>
                <div class="lk-options-pos" @click="$emit('onAction', 'pos', options.id, 3); options.enabled = false;"></div>
                <div class="lk-options-pos" @click="$emit('onAction', 'pos', options.id, 4); options.enabled = false;"></div>
                <div class="lk-options-pos" @click="$emit('onAction', 'pos', options.id, 5); options.enabled = false;"></div>
                <div class="lk-options-pos" @click="$emit('onAction', 'pos', options.id, 6); options.enabled = false;"></div>

                
                <div class="lk-options-pos" @click="$emit('onAction', 'pos', options.id, 7); options.enabled = false;"></div>
                <div class="lk-options-pos" @click="$emit('onAction', 'pos', options.id, 8); options.enabled = false;"></div>
                <div class="lk-options-pos" @click="$emit('onAction', 'pos', options.id, 9); options.enabled = false;"></div>
                <div class="lk-options-pos" @click="$emit('onAction', 'pos', options.id, 10); options.enabled = false;"></div>
                <div class="lk-options-pos" @click="$emit('onAction', 'pos', options.id, 11); options.enabled = false;"></div>
                <div class="lk-options-pos" @click="$emit('onAction', 'pos', options.id, 12); options.enabled = false;"></div>
                <div class="lk-options-pos" @click="$emit('onAction', 'pos', options.id, 13); options.enabled = false;"></div>
                <div class="lk-options-pos" @click="$emit('onAction', 'pos', options.id, 14); options.enabled = false;"></div>
                <div class="lk-options-pos" @click="$emit('onAction', 'pos', options.id, 15); options.enabled = false;"></div>
            </div>
            <div class="lk-options-buttons">
                <div class="lk-options-name" >{{options.name}}</div>
                <div class="button" v-if="!options.active" @click="$emit('onAction', 'activate', options.id); options.enabled = false;">Активировать</div> 
                <div class="button" v-else-if="dis" @click="$emit('onAction', 'desactivate', options.id); options.enabled = false;">Деактивировать</div>
                <div class="button" @click="$emit('onAction', 'sex', options.id); options.enabled = false;">Сделать самкой</div>
                <div class="button" @click="$emit('onAction', 'delete', options.id); options.enabled = false;">Удалить</div>
                <div class="button" @click="options.enabled = false">Назад</div>
                <div>
                    <h3>Стоимость:</h3>
                    <p>Сменить пол - <span class="lk-options-price">{{price.sex}} DC</span></p>
                    <p>Телепортировать - <span class="lk-options-price">{{price.position}} DC</span></p>
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
                    <div class="lk-inventory-growth">Рост: {{dino.growth}}</div>
                </div>
                <div v-else>
                    <div class="lk-inventory-tittle">Слот</div>
                    <div class="lk-inventory-tittle">свободен</div>
                </div>
            </div>            
            <div class="lk-inventory-item lk-inventory-add" title="Добавить слот" @click="$emit('onAction', 'slot'); options.enabled = false;">+</div>
        </div>
    </div>
</template>

<script>
export default {
    props:['inventory', 'price', 'dis','server'],
    data() {
        return {
            options:{
                id: -1,
                name: "",
                enabled: false,
                active: false
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
            //window.console.log(dino);            
        }
    },
    mounted(){
        //window.console.log(this.inventory)
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
        height: 480px;
        position: absolute;
        top: 2.5%;
        left: 2.5%;
        background-color: rgba($clr_2, .8);
        transform: translateY(-105%);
        transition: all .3s;        
        display: flex;
        justify-content: space-around;
        align-items: center;
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
        :nth-child(2){                
            top: 41%;
            left: 24%;
        }
        :nth-child(3){                
            top: 51%;
            left: 21%;
        }
        :nth-child(4){                
            top: 66%;
            left: 37%;
        }
        :nth-child(5){                
            top: 55%;
            left: 82%;
        }
        :nth-child(6){                
            top: 46%;
            left: 52%;
        }
        :nth-child(7){                
            top: 32%;
            left: 55%;
        }        
        :nth-child(8){                
            top: 8%;
            left: 43%;
        }

        
        :nth-child(9){                
            top: 27%;
            left: 13%;
        }
        :nth-child(10){                
            top: 28%;
            left: 28%;
        }
        :nth-child(11){                
            top: 17%;
            left: 41%;
        }
        :nth-child(12){                
            top: 17%;
            left: 55%;
        }
        :nth-child(13){                
            top: 60%;
            left: 58%;
        }
        :nth-child(14){                
            top: 39%;
            left: 38%;
        }
        :nth-child(15){                
            top: 70%;
            left: 25%;
        }
        :nth-child(16){                
            top: 54%;
            left: 34%;
        }
        :nth-child(17){                
            top: 34%;
            left: 71%;
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
    
}
</style>