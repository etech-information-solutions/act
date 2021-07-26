import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { IonicModule } from '@ionic/angular';

import { PodmanagementPageRoutingModule } from './podmanagement-routing.module';

import { PodmanagementPage } from './podmanagement.page';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    IonicModule,
    PodmanagementPageRoutingModule
  ],
  declarations: [PodmanagementPage]
})
export class PodmanagementPageModule {}
